using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VKPostman.Services;

namespace VKPostman.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => "/help";

        public override async void ExecuteAsync(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;
            await client.SendTextMessageAsync(chatId, "Справка будет когда-то потом.");
    
           
        }
    }
}
