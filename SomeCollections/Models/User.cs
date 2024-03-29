﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SomeCollections.Models
{
    public class User: IdentityUser
    {
        public virtual ICollection<Collection> ListCollections { get; set; }
        public virtual ICollection<Item> ListItems { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
