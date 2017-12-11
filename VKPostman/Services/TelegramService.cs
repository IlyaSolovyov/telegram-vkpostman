using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using VKPostman.DAL;
using VKPostman.Models;
using Microsoft.EntityFrameworkCore;
using VkNet.Model;
using VkNet.Model.Attachments;

namespace VKPostman.Services
{
    public class TelegramService
    {
        static TelegramBotClient telegram;
        static DatabaseContext db;

        static TelegramService()
        {
            telegram = Bot.Get().Result;
            db = new DatabaseContext();
        }

        #region Private Methods
        private static List<PublicPage> GetTrackedPages()
        {
            return db.PublicPages.ToList<PublicPage>();
        }
        private static List<Subscriber> GetSubscribers(PublicPage page)
        {
                List<Subscriber> subscribers = new List<Subscriber>();

                List<Subscription> subscriptions = db.Subscriptions
                .Include(sub => sub.Subscriber)
                .Where(subscription => subscription.PublicPageId == page.Id)               
                .ToList<Subscription>();

                foreach (var subscription in subscriptions)
                {
                    subscribers.Add(subscription.Subscriber);
                }
            return subscribers;
        }

        private static List<Post> GetNewPosts(PublicPage page)
        {
            return VkService.GetLastPosts(page.PageVkId, 50)
                            .Where(i => i.Id>page.LastPostId)
                            .OrderBy(o => o.Id)
                            .ToList<Post>();
        }
        private static async Task SendPostsAsync(Subscriber subscriber, List<Post> posts)
        {
            foreach(var post in posts)
            {
                await DeliverPostAsync(subscriber.ChatId, post);
              
            }
        }

        private static async Task DeliverPostAsync(long chatId, Post post)
        {
            if (post.Text.Length > 0)
            {
                await telegram.SendTextMessageAsync(373499493, post.Text.ToString());
                await telegram.SendTextMessageAsync(chatId, post.Text.ToString());
            }
            else if (post.Attachments.Count > 0)
            {
                await telegram.SendTextMessageAsync(373499493, "Хороший человек под номером " + chatId 
                    + " не получил свой пост, потому что там нет текста. Надеюсь, ты скоро это починишь.");
                await telegram.SendTextMessageAsync(chatId, "У этого поста какие-то картинки или музыка или вообще непонятно что. Я это ещё не сделал. Простите.");
            }
        }
        #endregion

        internal static async Task DeliverMessagesAsync()
        {
            List<PublicPage> pages = GetTrackedPages();
            foreach(var page in pages)
            {
                List<Subscriber> subscribers = GetSubscribers(page);
                if(subscribers.Count>0)
                {
                    List<Post> posts = GetNewPosts(page);
                    if (posts.Count > 0)
                    {
                        foreach (var subscriber in subscribers)
                        {
                            await SendPostsAsync(subscriber, posts);
                        }
                    }                
                }
            }
                      
        }
   
    }
}
