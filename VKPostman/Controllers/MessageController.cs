using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using VKPostman.Models;
using Telegram.Bot;
using VKPostman.Models.Commands;

namespace VKPostman.Controllers
{
    //[Produces("application/json")]
    [Route("api/message")]
    public class MessageController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]Update update)
        {
            var commands = Bot.Commands;
            var message = update.Message;
            var client = await Bot.Get();

            await RespondAsync(client, message, commands);
            return Ok("Response  has been issued");
        }

        private async Task RespondAsync(TelegramBotClient client, Message message, IReadOnlyList<Command> commands)
        {
            if (message.Sticker != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Не присылайте мне стикеры. Мне от них плохеет");
            }
            else if (message.Photo != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Хорошее изображение, но мне оно не нужно.");
            }
            else if (message.Video != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Я экономлю мобильный трафик. Я не смотрю видео.");
            }
            else if (message.Audio != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Прошлый альбом был так себе. Я не буду это слушать.");
            }
            else if (message.Contact != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Я не знакомлюсь по интернету, спасибо.");
            }
            else if (message.Document != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Это вирусы? Я воздержусь от скачивания.");
            }
            else if (message.Game != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "У меня нет времени на игры.");
            }
            else if (message.Location != null)
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Хорошее место, соглашусь.");
            }
            else
            {
                foreach (var command in commands)
                {
                    if (command.Contains(message.Text))
                    {
                        command.ExecuteAsync(message, client);
                        break;
                    }
                }
            }
        }

        [HttpGet]
        public IActionResult Pinch()
        {          
            return Ok("I'm not sleeping!");
        }
    }
}