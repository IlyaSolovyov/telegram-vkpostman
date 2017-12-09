using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using VKPostman.Models;

namespace VKPostman.Controllers
{
    //[Produces("application/json")]
    [Route("api/message")]
    public class MessageController : Controller
    {
        [HttpPost]
        public async  Task<IActionResult> Update([FromBody]Update update)
        {
            var commands = Bot.Commands;
            var message = update.Message;
            var client = await Bot.Get();

            foreach(var command in commands)
            {
                if (command.Contains(message.Text))
                {
                    command.Execute(message, client);
                    break;
                }
            }

            return Ok("Response to '" + message + "' command has been issued");
        }

        [HttpGet]
        public IActionResult Test()
        {          
            return Ok("Don't worry, routing works");
        }
    }
}