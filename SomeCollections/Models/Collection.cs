using System;
using System.Collections.Generic;

namespace SomeCollections.Models
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImgPath { get; set; }
        public int CountItems { get; set; }

        public virtual User Owner { get; set; }
        public virtual Tag Tag { get; set; }

        public virtual ICollection<Item> Items { get; set; }
        
    }
}
