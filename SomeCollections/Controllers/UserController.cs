using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomeCollections.Models;
using SomeCollections.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationContext _db;

        public UserController(ApplicationContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            ViewData["Title"] = "Личный кабинет";
            var Items = _db.Items.Where(p => p.User.UserName == User.Identity.Name);
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
            Item item = new Item
            {
                Name = model.Name,
                Description = model.Description,
                UserName = User.Identity.Name,
                User = _db.Users.FirstOrDefault(p => p.UserName == User.Identity.Name),
            };
            _db.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
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
