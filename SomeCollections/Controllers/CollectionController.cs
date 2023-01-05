using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SomeCollections.Models;
using SomeCollections.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly ApplicationContext _db;
        IWebHostEnvironment _appEnvironment;

        public CollectionController(ApplicationContext db, IWebHostEnvironment appEnvironment)
        {
            _db = db;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index(SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Collection> collections = _db.Collections.Include(x => x.Tag).Include(s => s.Owner).Where(p => p.Owner.UserName == User.Identity.Name);

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CountSort"] = sortOrder == SortState.CountAsc ? SortState.CoountDesc : SortState.CountAsc;

            collections = sortOrder switch
            {
                SortState.NameDesc => collections.OrderByDescending(s => s.Name),
                SortState.CountAsc => collections.OrderBy(s => s.CountItems),
                SortState.CoountDesc => collections.OrderByDescending(s => s.CountItems),
                _ => collections.OrderBy(s => s.Name),
            };
            
            return View(collections);
        }

        [AllowAnonymous]
        public IActionResult AllCollections(int? tag, string name,SortState sortOrder = SortState.NameAsc)
        {
            IQueryable<Collection> collections = _db.Collections.Include(p => p.Owner).Include(t=>t.Tag).Include(x => x.Tag);

            if( tag != null && tag != 0)
            {
                collections = collections.Where(p => p.Tag.Id == tag);
            }
            if (!String.IsNullOrEmpty(name))
            {
                collections = collections.Where(p => p.Name.Contains(name) || p.Description.Contains(name));
            }

            List<Tag> tags = _db.Tags.ToList();

            tags.Insert(0, new Tag { Id = 0, Name = "Все" });

            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CountSort"] = sortOrder == SortState.CountAsc ? SortState.CoountDesc : SortState.CountAsc;
            ViewData["OwnerSort"] = sortOrder == SortState.OwnerAsc ? SortState.OwnerDesc : SortState.OwnerAsc;

            collections = sortOrder switch
            {
                SortState.NameDesc => collections.OrderByDescending(s => s.Name),
                SortState.CountAsc => collections.OrderBy(s => s.CountItems),
                SortState.CoountDesc => collections.OrderByDescending(s => s.CountItems),
                SortState.OwnerAsc => collections.OrderBy(s => s.Owner.UserName),
                SortState.OwnerDesc => collections.OrderByDescending(s => s.Owner.UserName),
                _ => collections.OrderBy(s => s.Name),
            };

            FilterCollectionsViewModel viewModel = new FilterCollectionsViewModel
            {
                Collections = collections.ToList(),
                Tags = new SelectList(tags, "Id", "Name"),
                Name = name,
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Themes = _db.Tags.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CollectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Collection item = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Owner = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
                    Tag = _db.Tags.FirstOrDefault(p => p.Id == model.Tag),
                    CountItems = 0,
                };
                item.ImgPath = await UploadImg(model.Img);

                _db.Add(item);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            ViewBag.Themes = _db.Tags.ToList();
            return View(model);
        }

        private async Task<string> UploadImg (IFormFile uploadedFile)
        {
            string defaultPath = "/pic/default.jpg";

            if (uploadedFile == null)
                return defaultPath;

            string path = "/imgCollections/" + uploadedFile.FileName;

            FileInfo fileInf = new FileInfo(_appEnvironment.WebRootPath + path);
            nint temp = 0;

            while (fileInf.Exists)
            {
                fileInf = new FileInfo(_appEnvironment.WebRootPath + path);
                path = "/imgCollections/" + temp + uploadedFile.FileName;
                temp++;
            }

            using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }

            return path;
        }

        [HttpGet]
        public IActionResult CurrentCollectuin(Guid id)
        {
            ViewBag.Name = _db.Collections.FirstOrDefault(p => p.Id == id).Name;
            var collection = _db.Collections.Include(o=>o.Owner).Where(p => p.Id == id);
            return View(collection);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var collection = _db.Collections.FirstOrDefault(p => p.Id == id);
            ViewBag.Name = collection.Name;
            ViewBag.ColId = collection.Id;
            CollectionsViewModel colView = new CollectionsViewModel
            {
                Name = collection.Name,
                Description = collection.Description,
            };
            ViewBag.Themes = _db.Tags.ToList();
            return View(colView);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, CollectionsViewModel model)
        {
            var s = _db.Collections.FirstOrDefault(p => p.Id == id);
            s.Name = model.Name;
            s.Description = model.Description;
            s.Tag = _db.Tags.FirstOrDefault(p => p.Id == model.Tag);
            s.ImgPath = await UploadImg(model.Img);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid id, string returnUrl)
        {
            Collection col = _db.Collections.FirstOrDefault(p => p.Id == id);
            var items = _db.Items.Include(x=>x.Likes).Include(x=>x.Messages).Where(p => p.Collection.Id == id).ToList();

            if (col == null)
            {
                return NotFound();
            }

            foreach(var x in items)
            {
                string path = _appEnvironment.WebRootPath + x.ImgPath;
                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                    fileInf.Delete();
                _db.Items.Remove(x);
            }

            string path1 = _appEnvironment.WebRootPath + col.ImgPath;
            FileInfo fileInf1 = new FileInfo(path1);
            if (fileInf1.Exists)
                fileInf1.Delete();
            _db.Collections.Remove(col);

            await _db.SaveChangesAsync();
            return LocalRedirect(returnUrl);
        }
    }
}
