using System.Collections.Generic;
using System.Threading;

namespace Splash.Configs
{
    public class Config
    {
        public Discord discord { get; set; }
        public Twitch twitch { get; set; }
    }

    public class Discord
    {
        public string token { get; set; }
    }

    public class Twitch
    {
        public Authentication authentication { get; set; }
        public StreamMonitor streamMonitor { get; set; }
    }

    public class Authentication
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class StreamMonitor
    {
        public IList<MonitoredChannels> monitoredChannels { get; set; }
    }

    public class MonitoredChannels
    {
        public MonitoredChannels(string twitchChannel, ulong GuildID, ulong ChannelID)
        {
            this.twitchChannel = twitchChannel;
            this.GuildID = GuildID;
            this.ChannelID = ChannelID;
        }
        public string twitchChannel { get; set; }
        public ulong GuildID { get; set; }
        public ulong ChannelID { get; set; }
    }
}
