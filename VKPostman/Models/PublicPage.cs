using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKPostman.Models
{
    public class PublicPage
    {
        public int Id { get; set; }
        public long PageVkId { get; set; }
        public long LastPostId { get; set; }

        public List<Subscription> Subscriptions { get; set; }

        public PublicPage()
        {
            Subscriptions = new List<Subscription>();
        }
    }
}
