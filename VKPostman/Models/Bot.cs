using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using VKPostman.Models.Commands;

namespace VKPostman.Models
{
    public static class Bot
    {
        private static TelegramBotClient client;
        private static List<Command> commandsList;
        public static IReadOnlyList<Command> Commands { get => commandsList.AsReadOnly(); }

        public static async Task<TelegramBotClient> Get()
        {
            if (client != null)
            {
                return client;
            }

            commandsList = new List<Command>();
            commandsList.Add(new TestCommand());
            var hook = string.Format(AppSettings.Url, "api/message/");

            client = new TelegramBotClient(AppSettings.Key);
            await client.SetWebhookAsync(hook);
            return client;
        }
    }
}
