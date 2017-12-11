using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using VKPostman.Models;

namespace VKPostman.Services
{
    public class TelegramService
    {
        static TelegramBotClient client;

        static TelegramService()
        {
            client = Bot.Get().Result;
        }
   
        internal static async Task DeliverMessagesAsync()
        {
            await client.SendTextMessageAsync(373499493, "Привет, я бы сейчас постучался в паблики");
        }
    }
}
