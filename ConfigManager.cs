using Newtonsoft.Json;
using Splash.Configs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            //return configs.Discord.Token;
            return Environment.GetEnvironmentVariable("Token");
        }
        public static List<string> GetTwitchAuth()
        {
            return new List<string>() { configs.Twitch.Authentication.ClientId, configs.Twitch.Authentication.ClientSecret };
        }

        public static List<MonitoredChannel> GetTwitchMonitoredChannels()
        {
            return configs.Twitch.StreamMonitor.MonitoredChannels?.ToList() ?? null;
        }

        //public static bool SetNewStreamMonitor(string TwitchChannel, ulong GuildID, ulong ChannelID)
        //{
        //    //in case our config.json list is empty; maybe should initialize it somewhere else
        //    //if (configs.twitch.StreamMonitor.MonitoredChannels == null)
        //    //{
        //    //    configs.twitch.StreamMonitor.MonitoredChannels = new List<MonitoredChannel>();

        //    //    //we can safely add the stream since we know for sure it's not there
        //    //    configs.twitch.StreamMonitor.MonitoredChannels.Add(new MonitoredChannel(TwitchChannel, GuildID, ChannelID));
        //    //    StreamAdded?.Invoke(TwitchChannel);

        //    //    return true;
        //    //}

        //    ////we iterate through the list of GuidIDs
        //    //bool newItem = true;
        //    //foreach (MonitoredChannel mc in configs.twitch.StreamMonitor.MonitoredChannels.Where(c => c.GuildID == GuildID))
        //    //{
        //    //    //do we find our channel?
        //    //    if (mc.TwitchChannel == TwitchChannel)
        //    //    {
        //    //        newItem = false;
        //    //    }
        //    //}

        //    //if (newItem)
        //    //{
        //    //    configs.twitch.StreamMonitor.MonitoredChannels.Add(new MonitoredChannel(TwitchChannel, GuildID, ChannelID));
        //    //    StreamAdded?.Invoke(TwitchChannel);

        //    //    return true;
        //    //}

        //    //return false;

        //    //is the channel already being monitored? if not, then we add it to the list first
        //    if(configs.twitch.StreamMonitor.MonitoredChannels.Where(name => name.TwitchChannel == TwitchChannel).Count() == 0)
        //    {
        //        configs.twitch.StreamMonitor.MonitoredChannels.Add(new MonitoredChannel(TwitchChannel));
        //    }

        //    //is this guild already monitoring this twitch channel?

        //}

        public static void SetStreamLiveStatus(bool LiveStatus, string TwitchChannel)
        {
            //TwitchChannel = TwitchChannel.ToLower();

            //var monitoredChannel = configs.twitch.StreamMonitor.MonitoredChannels.Where(c => c.TwitchChannel == TwitchChannel).ToList();
            //for (int i = 0; i < monitoredChannel.Count(); i++)
            //{
            //    monitoredChannel[i].Live = LiveStatus;
            //    configs.twitch.StreamMonitor.MonitoredChannels[i] = monitoredChannel[i];
            //}
        }

        //public static bool GetStreamLiveStatus(string TwitchChannel)
        //{

        //}

        public static void ResetAllStreamLiveStatus(bool LiveStatus = false)
        {
            //for(int i = 0; i < configs.twitch.StreamMonitor.MonitoredChannels.Count; i++)
            //{
            //    configs.twitch.StreamMonitor.MonitoredChannels[i].Live = LiveStatus;
            //}
        }
    }
}
