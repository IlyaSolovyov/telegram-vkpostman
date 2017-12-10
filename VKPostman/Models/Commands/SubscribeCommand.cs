using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VKPostman.DAL;
using VKPostman.Services;

namespace VKPostman.Models.Commands
{
    public class SubscribeCommand : Command
    {
        public override string Name => "/subscribe";

        public override async void ExecuteAsync(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            string response = VkService.AddSubscription(chatId, message.Text);
            await client.SendTextMessageAsync(chatId, response);
            
     
        }
    }
}
