using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Splash
{
    static class Config
    {
        static IConfigurationRoot config;
        public static void Init()
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("config.json").Build();
        }

        public static string GetDiscordToken()
        {
            return config["discord:token"];
        }
        public static List<string> GetTwitchAuth()
        {
            var auth = new List<string>();
            auth.Add(config["twitch:authentication:client_id"]);
            auth.Add(config["twitch:authentication:client_secret"]);

            return auth;
        }

        public static List<string> GetTwitchMonitoredChannels()
        {
            return config.GetSection("twitch:stream_monitor").GetChildren().ToArray().Select(c => c.Value).ToList();
        }

        public static void SetNewStreamMonitor(string channel)
        {
            config["stream_monitor"] += channel;
        }
    }
}
