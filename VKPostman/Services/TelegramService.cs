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
using System.Text;

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
        private static async Task SendPostsAsync(Subscriber subscriber, Group group, List<Post> posts)
        {
            foreach(var post in posts)
            {
                await DeliverPostAsync(subscriber.ChatId,group, post);
              
            }
        }

        public static string GetPostInfo(Group page, Post post)
        {
            return String.Format("#{0} | Ссылка на пост: https://vk.com/wall-{1}_{2}\n", page.ScreenName, page.Id, post.Id);
        }
        public static string GetPostText(Post post)
        {
            return post.Text.Length!=0 ? post.Text + "\n" : null;
        }
        public static string GetPostContent(Post post)
        {
            int photoCount = 0;
            int audioCount = 0;
            int videoCount = 0;

            StringBuilder contentBuilder = new StringBuilder();
            foreach(var attachment in post.Attachments)
            {
                if (attachment.Type == typeof(Photo))
                {
                    photoCount++;
                }
                else if (attachment.Type == typeof(Audio))
                {
                    audioCount++;
                }
                else if (attachment.Type == typeof(Video))
                {
                    videoCount++;
                }
            }
            if (photoCount != 0) contentBuilder.Append("🎨 Количество изображений: " + photoCount + "\n");
            if (audioCount != 0) contentBuilder.Append("🎧 Количество аудиозаписей: " + audioCount + "\n");
            if (videoCount != 0) contentBuilder.Append("🎬 Количество видеозапией: " + videoCount + "\n\n");
            return contentBuilder.ToString();

        }
        public static string GetPostTelegraph(Post post)
        {
            return TelegraphService.GetTelegraphPage(post);
        }

        private static string PrepareMessage(Group group, Post post)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(GetPostInfo(group, post));
            messageBuilder.AppendLine(GetPostText(post));
            messageBuilder.AppendLine(GetPostContent(post));
            messageBuilder.AppendLine(GetPostTelegraph(post));
            return messageBuilder.ToString();
        }
        private static async Task DeliverPostAsync(long chatId, Group group, Post post)
        {
            await telegram.SendTextMessageAsync(chatId, PrepareMessage(group, post));          
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
                            await SendPostsAsync(subscriber, VkService.GetPageByScreenName(page.ScreenName), posts);
                        }
                        page.LastPostId = posts.Max(p => p.Id).Value;
                        db.SaveChanges();
                    }                
                }
            }
                      
        }  
    }
}
