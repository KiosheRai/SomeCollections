using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SomeCollections.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
            List<Collection> collection = _db.Collections.Include(x=>x.Items).Where(x=>x.Owner.Id == user.Id).ToList();
            List<Like> likes = _db.Likes.Where(x => x.User.Id == user.Id).ToList();
            List<Message> messages = _db.Messages.Where(x => x.Sender.Id == user.Id).ToList();

            if (user == null)
            {
                return NotFound();
            }

            foreach (var col in collection)
            {
                var items = _db.Items.Include(x => x.Likes).Include(x => x.Messages).Where(p => p.Collection.Id == col.Id).ToList();

                _db.Items.RemoveRange(items);

                _db.Collections.Remove(col);
            }

            _db.Likes.RemoveRange(likes);
            _db.Messages.RemoveRange(messages);

            await _db.SaveChangesAsync();
            IdentityResult result = await _userManager.DeleteAsync(user);
            
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