using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SomeCollections.Models
{
    public class User: IdentityUser
    {
        public virtual ICollection<Collection> ListCollections { get; set; }
        public virtual ICollection<Item> ListItems { get; set; }
    }
}
