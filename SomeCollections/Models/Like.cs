using System;

namespace SomeCollections.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        public virtual User UserId { get; set; }
        public virtual Item ItemId { get; set; }
    }
}
