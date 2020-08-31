using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Twitch.Net;
using Twitch.Net.Response;

namespace Splash
{
    public static class TwitchManager
    {
        static Timer LiveCheckTimer;
        static TwitchApi twitchApi;

        public async static Task Init()
        {
            var keypair = ConfigParser.GetTwitchAuth();
            twitchApi = new TwitchApiBuilder(keypair[0]).
                WithClientSecret(keypair[1]).
                Build();

            LiveCheckTimer = new Timer(180000);
            LiveCheckTimer.Elapsed += LiveCheckTimer_Elapsed;
            LiveCheckTimer.Enabled = true;
        }

        private static void LiveCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            twitchApi.GetStreamsWithUserLogins(ConfigParser.GetTwitchMonitoredChannels().ToArray());


        }
    }
}