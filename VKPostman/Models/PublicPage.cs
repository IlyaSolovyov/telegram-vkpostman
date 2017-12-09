using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKPostman.Models
{
    public class PublicPage
    {
        public int Id { get; set; }
        public int PageVkId { get; set; }
        public int LastPostId { get; set; }

        public List<Subscription> Subscriptions { get; set; }

        public PublicPage()
        {
            Subscriptions = new List<Subscription>();
        }
    }
}
