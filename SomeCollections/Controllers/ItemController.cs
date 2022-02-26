using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomeCollections.Models;
using SomeCollections.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationContext _db;
        IWebHostEnvironment _appEnvironment;

        public ItemController(ApplicationContext db, IWebHostEnvironment appEnvironment)
        {
            _db = db;
            _appEnvironment = appEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index(Guid Id, SortState sortOrder = SortState.NameAsc)
        {
            ViewBag.Collection = _db.Collections.Include(s => s.Owner).FirstOrDefault(p => p.Id == Id);
            IQueryable<Item> items = _db.Items.Include(x => x.Owner).Where(p => p.Collection.Id == Id);

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["LikeSort"] = sortOrder == SortState.CountAsc ? SortState.CoountDesc : SortState.CountAsc;

            items = sortOrder switch
            {
                SortState.NameDesc => items.OrderByDescending(s => s.Name),
                SortState.CountAsc => items.OrderBy(s => s.LikeCount),
                SortState.CoountDesc => items.OrderByDescending(s => s.LikeCount),
                _ => items.OrderBy(s => s.Name),
            };

            return View(items);
        }

        [AllowAnonymous]
        public IActionResult CurrentItem(Guid Id)
        {
            var item = _db.Items.Include(s=>s.Collection).Include(o=>o.Owner).FirstOrDefault(p => p.Id == Id);
            ViewBag.Comments = _db.Messages.Include(s=>s.Sender).Where(s => s.Item.Id == item.Id); 
            return View(item);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(Guid Id)
        {
            ViewBag.CollectionId = Id;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(ItemViewModel model, Guid Id)
        {
            if (ModelState.IsValid)
            {
                Collection col = _db.Collections.FirstOrDefault(p=>p.Id == Id);
                col.CountItems += 1;
                Item item = new Item
                {
                    Name = model.Name,
                    Description = model.Description,
                    Owner = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
                    Collection = _db.Collections.FirstOrDefault(p => p.Id == Id),
                };
                item.ImgPath = await UploadImg(model.Img);

                _db.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Item", new { Id });
            }
            ViewBag.CollectionId = Id;
            return View(model);
        }


        private async Task<string> UploadImg(IFormFile uploadedFile)
        {
            string path = null;
            if (uploadedFile != null)
            {
                path = "/imgItems/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            else
            {
                path = "/imgItems/default.jpg";
            }

            return path;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(Guid id)
        {
            var item = _db.Items.Include(x=>x.Collection).FirstOrDefault(p => p.Id == id);
            ViewBag.Name = item.Name;
            ViewBag.ItemId = item.Id;
            ViewBag.ColId = item.Collection.Id;
            ItemViewModel colView = new ItemViewModel
            {
                Name = item.Name,
                Description = item.Description,
            };
            return View(colView);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, ItemViewModel model)
        {
            Item s = _db.Items.Include(x=>x.Collection).FirstOrDefault(p => p.Id == id);
            s.Name = model.Name;
            s.Description = model.Description;
            s.ImgPath = await UploadImg(model.Img);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Item", new { s.Collection.Id });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            
            Item item = _db.Items.Include(x=>x.Collection).Include(x=>x.Likes).Include(x=>x.Messages).FirstOrDefault(p => p.Id == id);
            Collection col = _db.Collections.FirstOrDefault(p=>p.Id == item.Collection.Id);
            
            if (item != null && col != null)
            {
                col.CountItems -= 1;
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Item", new { col.Id });
        }
    }
}
