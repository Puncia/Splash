using System.Collections.Generic;

namespace Splash.Configs
{
    public class Config
    {
        public Discord Discord { get; set; }
        public Twitch Twitch { get; set; }
    }

    public class Discord
    {
        public string Token { get; set; }
    }

    public class Twitch
    {
        public Authentication Authentication { get; set; }
        public StreamMonitor StreamMonitor { get; set; }
    }

    public class Authentication
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class StreamMonitor
    {
        public IList<MonitoredChannel> MonitoredChannels { get; set; }
        public IList<GuildPreference> GuildPreferences { get; set; }
    }

    public class MonitoredChannel
    {
        public MonitoredChannel(string twitchChannel)
        {
            this.TwitchChannel = twitchChannel;
            this.Live = false;
        }

        public string TwitchChannel { get; set; }
        public bool Live { get; set; }
    }
    public class GuildPreference
    {
        public ulong GuildID { get; set; }
        public ulong TwitchNotificationChannelID { get; set; }
    }
}
