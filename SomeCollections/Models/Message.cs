using System;

namespace SomeCollections.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        public virtual User Sender { get; set; }
        public virtual Item Item { get; set; }
    }
}
