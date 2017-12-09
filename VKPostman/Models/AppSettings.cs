﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKPostman.Models
{
    public static class AppSettings
    {
        public static string Url { get; set; } = "https://e28d7c48.ngrok.io/{0}";
        public static string Name { get; set; } = "vkpostmann_bot";
        public static string VkApiKey { get; set; }
        public static string BotApiKey { get; set; }
    }
}