using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Splash.Configs;

namespace Splash
{
    static class ConfigManager
    {
        const string configFile = "config.json";
        static Config configs;

        public static void Init()
        {
            configs = new Config();
            configs.discord = new Configs.Discord();
            configs.twitch = new Configs.Twitch();
            configs.twitch.authentication = new Configs.Authentication();
            configs.twitch.streamMonitor = new Configs.StreamMonitor();

            LoadSettings();
        }

        public static void LoadSettings()
        {
            if (File.Exists(configFile))
            {
                string json = File.ReadAllText(configFile);

                configs = JsonConvert.DeserializeObject<Config>(json);
            }
            else
                return;
        }
        
        public static void SaveSettings(string s)
        {
            if(File.Exists(configFile))
            {
                File.WriteAllText(configFile, s);
            }
        }

        public static string GetDiscordToken()
        {
            return configs.discord.token;
        }
        public static List<string> GetTwitchAuth()
        {
            return new List<string>() { configs.twitch.authentication.client_id, configs.twitch.authentication.client_secret };
        }

        public static List<string> GetTwitchMonitoredChannels()
        {
            return new List<string>(configs.twitch.streamMonitor.streams.ToList());
        }

        public static void SetNewStreamMonitor(string channel)
        {
            configs.twitch.streamMonitor.streams.Add(channel);
            SaveSettings(JsonConvert.SerializeObject(configs));
        }
    }
}
