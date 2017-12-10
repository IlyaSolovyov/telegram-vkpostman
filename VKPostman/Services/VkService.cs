using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VKPostman.DAL;
using VKPostman.Models;

namespace VKPostman.Services
{
    public class VkService
    {
        static VkApi client;
        static DatabaseContext db;
        static VkService()
        {
            client = new VkApi();
            db = new DatabaseContext();
            client.Authorize(new ApiAuthParams { AccessToken = AppSettings.VkApiKey});
        }

        #region Private Methods
        static string TryParseCommand(string command)
        {
            string parameter = command.Split(' ').Last();
            if (parameter == null)
            {
                return "Ошибка : формат команды должен быть /subscribe https://vk.com/идентификаторпаблика.";
            } else if (!parameter.StartsWith("https://vk.com/"))
            {
                return "Ошибка : формат ссылки должен быть https://vk.com/идентификаторпаблика";
            }
            return null;
        }
        static string ParseCommand(string command)
        {
            return command.Split('/').Last();
        }
               
        static string TrySubscribe(long chatId, Group page)
        {
            if (page == null)
            {
                return "Ошибка параметра: данная страница не существует.";
            }
            else if (page.Type!= VkNet.Enums.SafetyEnums.GroupType.Page)
            {
                return "Ошибка параметра: данная страница не является пабликом.";
            }
            else
            {
                Subscriber subscriberModel = AddOrReturnSubscriber(chatId);
                PublicPage pageModel = AddOrReturnPublicPage(page);
                if (db.Subscriptions.Any(subscription => subscription.PublicPageId == pageModel.Id))
                {
                    return "Ошибка параметра: вы уже подписаны на данную страницу.";
                }
            }
            return null;
        }
        static string Subscribe(long chatId, Group page)
        {
            Subscriber subscriberModel = AddOrReturnSubscriber(chatId);
            PublicPage pageModel = AddOrReturnPublicPage(page);
            subscriberModel.Subscriptions.Add(new Subscription { PublicPageId = pageModel.Id, SubscriberId = subscriberModel.Id });
            db.SaveChanges();
            return "Вы успешно подписались на '" + page.Name + "'.";
        }

        public static Group GetPageByScreenName(string pageScreenName)
        {
            try
            {
                return client.Groups.GetById(pageScreenName);
            }
            catch (VkNet.Exception.ParameterMissingOrInvalidException ex)
            {
                return null;
            }
           
        }
        static long GetLastPostId(long pageId)
        {
            pageId *= -1;
            var twoLastPosts = client.Wall.Get(new WallGetParams
            {
                Count = 2,
                OwnerId = pageId
            });
            return twoLastPosts.WallPosts.Max(post => post.Id).Value;
        }

        static Subscriber AddOrReturnSubscriber(long chatId)
        {
            Subscriber subscriber;
            if (!db.Subscribers.Any(o => o.ChatId == chatId))
            {
                subscriber = new Subscriber()
                {
                    ChatId = chatId
                };
                db.Subscribers.Add(subscriber);
                db.SaveChanges();
            }
            else
            {
                subscriber = db.Subscribers.Where(o => o.ChatId == chatId).First();
            }
            return subscriber;
        }
        static PublicPage AddOrReturnPublicPage(Group group)
        {
            PublicPage page;
            if (!db.PublicPages.Any(o => o.PageVkId == group.Id))
            {
                page = new PublicPage()
                {
                    PageVkId = group.Id,
                    LastPostId = GetLastPostId(group.Id)
                };
                db.PublicPages.Add(page);
                db.SaveChanges();
            }
            else
            {
                page = db.PublicPages.Where(o => o.PageVkId == group.Id).First();
            }
            return page;

        }
        #endregion

        #region Public Commands
        public static string AddSubscription(long chatId, string message)
        {
            string botResponse = TryParseCommand(message);
            if (botResponse == null)
            {
                
                Group page = GetPageByScreenName(ParseCommand(message));
                botResponse = TrySubscribe(chatId, page);
                if (botResponse == null)
                {
                    botResponse = Subscribe(chatId, page);
                }
            }
            return botResponse;
        }
        #endregion
    }
}
