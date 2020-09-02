using Newtonsoft.Json;
using Splash.Configs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Splash
{
    static class ConfigManager
    {
        const string configFile = "config.json";
        static Config configs;

        public delegate void StreamAddedEventHandler(string stream);
        public static event StreamAddedEventHandler StreamAdded;

        public static void Init()
        {
            configs = new Config();

            StreamAdded += ConfigManager_StreamAdded;

            LoadSettings();
        }

        private static void ConfigManager_StreamAdded(string stream)
        {
            SaveSettings(JsonConvert.SerializeObject(configs));
        }

        public static void LoadSettings()
        {
            if (File.Exists(configFile))
            {
                string json = File.ReadAllText(configFile);

                configs = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                return;
            }
        }

        public static void SaveSettings(string s)
        {
            if (File.Exists(configFile))
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

        public static Dictionary<string, ulong> GetTwitchMonitoredChannels()
        {
            Dictionary<string, ulong> l = new Dictionary<string, ulong>();
            if (l.Count == 0)
            {
                return null;
            }

            foreach (MonitoredChannels monitored in configs.twitch.streamMonitor.monitoredChannels)
            {
                l.Add(monitored.twitchChannel, monitored.GuildID);
            }
            return l;
        }

        public static bool SetNewStreamMonitor(string twitchChannel, ulong GuildID, ulong ChannelID)
        {
            //in case our config.json list is empty; maybe should initialize it somewhere else
            if (configs.twitch.streamMonitor.monitoredChannels == null)
            {
                configs.twitch.streamMonitor.monitoredChannels = new List<MonitoredChannels>();

                //we can safely add the stream since we know for sure it's not there
                configs.twitch.streamMonitor.monitoredChannels.Add(new MonitoredChannels(twitchChannel, GuildID, ChannelID));
                StreamAdded?.Invoke(twitchChannel);

                return true;
            }

            //we iterate through the list of GuidIDs
            bool newItem = true;
            foreach (MonitoredChannels mc in configs.twitch.streamMonitor.monitoredChannels.Where(c => c.GuildID == GuildID))
            {
                //do we find our channel?
                if (mc.twitchChannel == twitchChannel)
                {
                    newItem = false;
                }
            }

            if(newItem)
            {
                configs.twitch.streamMonitor.monitoredChannels.Add(new MonitoredChannels(twitchChannel, GuildID, ChannelID));
                StreamAdded?.Invoke(twitchChannel);

                return true;
            }

            return false;
        }
    }
}
