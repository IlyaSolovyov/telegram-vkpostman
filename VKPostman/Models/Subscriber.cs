using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKPostman.Models
{
    public class Subscriber
    {
        public int Id { get; set; }
        public int ChatId { get; set; }

        public List<Subscription> Subscriptions { get; set; }

        public Subscriber()
        {
            Subscriptions = new List<Subscription>();
        }
    }
}
