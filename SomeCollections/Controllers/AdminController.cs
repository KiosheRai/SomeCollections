using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SomeCollections.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace CustomIdentityApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        ApplicationContext _db;
        UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager, ApplicationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.GetUsersInRoleAsync("user");
            return View(users);    
        }

        [Authorize]
        public async Task<IActionResult> Admins()
        {
            var users = await _userManager.GetUsersInRoleAsync("admin");
            return View(users);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<ActionResult> Blocking(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user.LockoutEnd == null)
            {
                user.LockoutEnd = System.DateTimeOffset.Now.AddYears(100);
            }
            else
            {
                user.LockoutEnd = null;
            }
            await _userManager.UpdateAsync(user);
            
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<ActionResult> ChangeRoleToUser(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await _userManager.RemoveFromRoleAsync(user, "admin");
                return RedirectToAction("Admins");
            }

            return NotFound();
        }

        [Authorize]
        public async Task<ActionResult> ChangeRoleToAdmin(Guid id)
        {
            User user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "admin");
                await _userManager.RemoveFromRoleAsync(user, "user");
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}