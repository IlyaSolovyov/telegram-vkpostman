using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKPostman.Models
{
    public class Subscription
    {
        public int SubscriberId { get; set; }
        public Subscriber Subscriber { get; set; }

        public int PublicPageId { get; set; }
        public PublicPage PublicPage { get; set; }
    }
}
