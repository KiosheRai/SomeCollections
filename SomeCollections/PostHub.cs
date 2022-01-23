using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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

        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Send", message);
        }

        //public async Task Like(Guid itemId, string UserName)
        //{
        //    Item item = _db.Items.FirstOrDefault(p=>)
        //    var checkedExistLike = _db.Likes.Where(p => p.ItemId == itemId && p.UserName == UserName).ToList().Count;
        //    if (checkedExistLike == 0)
        //    {
        //        Like like = new Like { UserName = UserName, ItemId = itemId };
        //        _db.Likes.Add(like);
        //        await _db.SaveChangesAsync();
        //    }
        //    var likes = _db.Likes.Where(p => p.ItemId == itemId).ToList().Count;
        //    await this.Clients.Group(itemId).SendAsync("getLike", likes);
        //}
    }
}
