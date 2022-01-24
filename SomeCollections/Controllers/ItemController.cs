using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SomeCollections.Models;
using SomeCollections.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private readonly ApplicationContext _db;

        public ItemController(ApplicationContext db)
        {
            _db = db;
        }
        [AllowAnonymous]
        public IActionResult Index(Guid Id)
        {
            var Collection = _db.Collections.FirstOrDefault(p => p.Id == Id);
            ViewData["Title"] = Collection.Name.ToString();
            ViewBag.Name = Collection.Name.ToString();
            ViewBag.CollectionId = Id;
            var Items = _db.Items.Include(x=>x.Owner).Where(p => p.Collection.Id == Id);
            return View(Items);
        }

        [AllowAnonymous]
        public IActionResult CurrentItem(Guid Id)
        {
            var item = _db.Items.Include(s=>s.Collection).Include(o=>o.Owner).FirstOrDefault(p => p.Id == Id);
            Collection col = _db.Collections.FirstOrDefault(p => p.Id == item.Collection.Id);
            ViewBag.NameCollection = col.Name;
            ViewBag.IdCollection = col.Id;
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
                _db.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Item", new { Id });
            }
            return View(model);
            
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
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Item", new { s.Collection.Id });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            
            Item item = _db.Items.Include(x=>x.Collection).FirstOrDefault(p => p.Id == id);
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
