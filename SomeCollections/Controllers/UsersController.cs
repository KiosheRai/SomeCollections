using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SomeCollections.Models;
using SomeCollections.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CustomIdentityApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        public IActionResult Index() => View(_userManager.Users.ToList());
        [Authorize]
        public IActionResult Create() => View();
        
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
        public async Task<ActionResult> ChangeRole(string id)
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
    }
}