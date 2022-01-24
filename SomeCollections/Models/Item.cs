using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomeCollections.Models
{
    public class Item
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual User Owner { get; set; }
        public virtual Collection Collection { get; set; }

        public int LikeCount { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
    }
}
