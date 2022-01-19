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
            var Items = _db.Collections.Where(p => p.Owner.UserName == User.Identity.Name);
            return View(Items);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCollectionsViewModel model)
        {
            Collection item = new Collection
            {
                Name = model.Name,
                Description = model.Description,
                UserName = User.Identity.Name,
                Owner = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
                CountItems = 0,
            };
            _db.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult CurrentCollectuin(Guid id)
        {
            ViewBag.Name = _db.Collections.FirstOrDefault(p => p.Id == id).Name;
            var collection = _db.Collections.Where(p => p.Id == id);
            return View(collection);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(Guid id)
        {
            Collection item = _db.Collections.FirstOrDefault(p => p.Id == id);

            if (item != null)
            {
                _db.Collections.Remove(item);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
