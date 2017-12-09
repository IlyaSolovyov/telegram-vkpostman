using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace VKPostman.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract void ExecuteAsync(Message message, TelegramBotClient client);

        public bool Contains(string command)
        {
            return command.Contains(this.Name); 
        }
    }
}
