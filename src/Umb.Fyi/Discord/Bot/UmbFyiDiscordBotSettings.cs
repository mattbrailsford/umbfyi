using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Umb.Fyi.Discord.Bot
{
    public class UmbFyiDiscordBotSettings
    {
        public UmbFyiDiscordBotSettings(IConfiguration config, IWebHostEnvironment hostingEnvironment)
        {
            Token = config.GetValue<string>("UmbFyi:Discord:Token");
            RootPath = hostingEnvironment.WebRootPath;
            BotEnabled = config.GetValue<string>("UmbFyi:Discord:Enabled") == "true";
        }

        public string Token { get; }
        public string RootPath { get; }
        public bool BotEnabled { get; set; }
    }
}
