using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace VKPostman.Models.Commands
{
    public class TestCommand : Command
    {
        public override string Name => "test";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var chatId = message.Chat.Id;
            var messageId = message.MessageId;

            await client.SendTextMessageAsync(chatId, "Ага, понятно.", replyToMessageId: messageId);
        }
    }
}
