using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize]
        public IActionResult Index(Guid Id)
        {
            var Collection = _db.Collections.FirstOrDefault(p => p.Id == Id);
            ViewData["Title"] = Collection.Name.ToString();
            ViewBag.Name = Collection.Name.ToString();
            ViewBag.CollectionId = Id;
            var Items = _db.Items.Where(p => p.Collection.Id == Id);
            return View(Items);
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
        public async Task<IActionResult> Create(CreateItemViewModel model, Guid Id)
        {
            if (ModelState.IsValid)
            {
                Collection col = _db.Collections.FirstOrDefault(p=>p.Id == Id);
                col.CountItems += 1;
                Item item = new Item
                {
                    Name = model.Name,
                    Description = model.Description,
                    UserName = User.Identity.Name,
                    Owner = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
                    Collection = _db.Collections.FirstOrDefault(p => p.Id == Id),
                };
                _db.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Item", new { Id });
            }
            return View(model);
            
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            Item item = _db.Items.FirstOrDefault(p => p.Id == id);

            if (item != null)
            {
                _db.Items.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
