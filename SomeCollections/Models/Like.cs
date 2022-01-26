using System;

namespace SomeCollections.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        public virtual User User { get; set; }
        public virtual Item Item { get; set; }
    }
}
