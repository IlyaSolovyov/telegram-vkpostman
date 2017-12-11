using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VKPostman.Models;
using System.Threading;
using System.Net.Http;
using FluentScheduler;
using VKPostman.Services;

namespace VKPostman
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PopulateAppSettings();
            ThreadPool.QueueUserWorkItem(o => Pinch());
            ScheduleNewsFeed();
            Bot.Get().Wait();        
        }

        private void Pinch()
        {
            HttpClient client = new HttpClient();
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(15));
                client.GetStringAsync(AppSettings.Url + "/api/message");
            }
        }

        private void ScheduleNewsFeed()
        {
            var registry = new Registry();
            registry.Schedule(() => TelegramService.DeliverMessagesAsync()).ToRunEvery(10).Seconds();
            JobManager.Initialize(registry);
        }


        private void PopulateAppSettings()
        {
            AppSettings.Url = Configuration["Url"];
            AppSettings.BotApiKey = Configuration["BotApiKey"];
            AppSettings.VkApiKey = Configuration["VkApiKey"];
            AppSettings.ConnectionString = Configuration["ConnectionString"];
            AppSettings.VkAppId = Configuration["VkAppId"];
            AppSettings.TelegraphApiKey = Configuration["TelegraphApikey"];
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
