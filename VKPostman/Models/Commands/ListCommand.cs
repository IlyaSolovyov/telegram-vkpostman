using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VKPostman.Services;

namespace VKPostman.Models.Commands
{
    public class ListCommand : Command
    {
        public override string Name => "/list";

        public override async void ExecuteAsync(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            string response = VkService.ListSubscriptions(chatId);
            await client.SendTextMessageAsync(chatId, response);
        }
    }
}
