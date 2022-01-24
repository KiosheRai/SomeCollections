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
    public class CollectionController : Controller
    {
        private readonly ApplicationContext _db;

        public CollectionController(ApplicationContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewData["Title"] = "Личный кабинет";
            var Items = _db.Collections.Include(x=>x.Tag).Include(s=>s.Owner).Where(p => p.Owner.UserName == User.Identity.Name);
            return View(Items);
        }

        [AllowAnonymous]
        public IActionResult AllCollections()
        {
            var item = _db.Collections.Include(p=>p.Owner).Include(x=>x.Tag).ToList();
            return View(item);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Themes = _db.Tags.ToList();
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CollectionsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Collection item = new Collection
                {
                    Name = model.Name,
                    Description = model.Description,
                    Owner = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
                    Tag = _db.Tags.FirstOrDefault(p=>p.Id == model.Tag),
                    CountItems = 0,
                };
                _db.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Themes = _db.Tags.ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CurrentCollectuin(Guid id)
        {
            ViewBag.Name = _db.Collections.FirstOrDefault(p => p.Id == id).Name;
            var collection = _db.Collections.Include(o=>o.Owner).Where(p => p.Id == id);
            return View(collection);
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, CollectionsViewModel model)
        {
            var s = _db.Collections.FirstOrDefault(p => p.Id == id);
            s.Name = model.Name;
            s.Description = model.Description;
            s.Tag = _db.Tags.FirstOrDefault(p => p.Id == model.Tag);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            Collection col = _db.Collections.FirstOrDefault(p => p.Id == id);
            var items = _db.Items.Where(p => p.Collection.Id == id).ToList();
            if (col == null)
            {
                return NotFound();
            }
            _db.Collections.Remove(col);
            foreach(var x in items)
            {
                _db.Items.Remove(x);
            }
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
