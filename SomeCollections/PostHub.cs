using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SomeCollections.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeCollections
{
    public class PostHub : Hub
    {
        ApplicationContext _db;
        UserManager<User> _userManager;

        public PostHub(ApplicationContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task SendComment(string message, string userName)
        {
            await this.Clients.All.SendAsync("Send", message, userName);
        }

        public async Task Like(Guid itemId, string UserName)
        {
            var checkedExistLike = _db.Likes
                .Include(p => p.Item)
                .Include(s => s.User)
                .Where(p => p.Item.Id == itemId && p.User.UserName == UserName)
                .ToList();

            var user = _db.Users.FirstOrDefault(s => s.UserName == UserName);
            var item = _db.Items.FirstOrDefault(g => g.Id == itemId);

            if (checkedExistLike.Count == 0)
            {
                Like like = new Like { User = user, Item = item };
                item.LikeCount += 1;
                _db.Likes.Add(like);
            }
            else
            {
                item.LikeCount -= 1;
                _db.Likes.Remove(checkedExistLike[0]);
            }
            await _db.SaveChangesAsync();
            await Clients.All.SendAsync("getLike", item.LikeCount);
        }
    }
}
