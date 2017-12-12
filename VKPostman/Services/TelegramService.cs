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
                            .Where(i => i.Id > page.LastPostId)
                            .OrderBy(o => o.Id)
                            .ToList<Post>();
        }
        private static async Task SendPostsAsync(Subscriber subscriber, PublicPage page, Group group, List<Post> posts)
        {
            foreach (var post in posts)
            {
                await DeliverPostAsync(subscriber.ChatId, group, post);
                page.LastPostId = post.Id.Value;
                db.SaveChanges();

            }
        }

        public static string GetPostInfo(Group page, Post post)
        {
            return String.Format("#{0} | Ссылка на пост: https://vk.com/wall-{1}_{2}\n", page.ScreenName, page.Id, post.Id);
        }
        public static string GetPostText(Post post)
        {

           if (post.Text.Length < 1024)
            {
                return post.Text.Length != 0 ? post.Text + "\n" : null;
            }
            else
            {
                StringBuilder result = new StringBuilder();
                string[] sentences = System.Text.RegularExpressions.Regex.Split(post.Text, @"(?<=[\.!\?])\s+");
                if (sentences.Length > 5)
                {                   
                    for (int i = 0; i < 5; i++)
                    {
                        result.Append(sentences[i]);
                    }
                }
                else
                {
                    result.Append(post.Text.Substring(0, 1024));
                }
                result.Append("\n(Продолжение текста по ссылке...)");
                return result.ToString();
            }         
        }
        public static string GetPostContent(Post post)
        {
            int photoCount = 0;
            int audioCount = 0;
            int videoCount = 0;

            StringBuilder contentBuilder = new StringBuilder();
            foreach (var attachment in post.Attachments)
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
        public static async Task<string> GetPostTelegraph(Group page, Post post)
        {
            return await TelegraphService.GetTelegraphPageAsync(page, post);
        }

        private static string PrepareMessage(Group group, Post post)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(GetPostTelegraph(group, post).Result + "\n");
            messageBuilder.AppendLine(GetPostInfo(group, post));
            messageBuilder.AppendLine(GetPostText(post));
            messageBuilder.AppendLine(GetPostContent(post));
            return messageBuilder.ToString();
        }
        private static async Task DeliverPostAsync(long chatId, Group group, Post post)
        {
            await telegram.SendTextMessageAsync(chatId, PrepareMessage(group, post));
        }

        #endregion

        internal static async Task DeliverMessagesAsync()
        {
            await telegram.SendTextMessageAsync(373499493, "Начало цикла.");
            int count = 0;
            List<PublicPage> pages = GetTrackedPages();
            foreach (var page in pages)
            {
                List<Subscriber> subscribers = GetSubscribers(page);
                if (subscribers.Count > 0)
                {
                    List<Post> posts = GetNewPosts(page);
                    if (posts.Count > 0)
                    {
                        foreach (var subscriber in subscribers)
                        {
                            await SendPostsAsync(subscriber, page, VkService.GetPageByScreenName(page.ScreenName), posts);                           
                        }
                        count += posts.Count*subscribers.Count;
                    }
                }
            }
                await telegram.SendTextMessageAsync(373499493, "Отправлено " + count + " сообщений.");
        }

        internal static async Task DeliverMessagesAsyncForDebug()
        {
            await telegram.SendTextMessageAsync(373499493, "Начало цикла.");
            List<PublicPage> pages = GetTrackedPages();
            foreach (var page in pages)
            {
                await telegram.SendTextMessageAsync(373499493, "Начало работы со страницей " + page.Name);
                List<Subscriber> subscribers = GetSubscribers(page);
                if (subscribers.Count > 0)
                {
                    await telegram.SendTextMessageAsync(373499493, "На неё подписаны " + subscribers.Count);
                    List<Post> posts = GetNewPosts(page);
                    if (posts.Count > 0)
                    {
                        await telegram.SendTextMessageAsync(373499493, "Количество новых постов: " + posts.Count);
                        foreach (var subscriber in subscribers)
                        {
                            await SendPostsAsync(subscriber, page, VkService.GetPageByScreenName(page.ScreenName), posts);
                        }
                    }
                }

            }


        }
    }
}
