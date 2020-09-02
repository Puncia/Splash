using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Twitch.Net;
using Twitch.Net.Models;

namespace Splash
{
    public static class TwitchManager
    {
        static Timer LiveCheckTimer;
        static TwitchApi twitchApi;

        public async static Task Init()
        {
            var keypair = ConfigManager.GetTwitchAuth();
            twitchApi = new TwitchApiBuilder(keypair[0]).
                WithClientSecret(keypair[1]).
                Build();

            LiveCheckTimer = new Timer(5000); //3 minutes = 180000; TODO: put this in config.json
            LiveCheckTimer.Elapsed += LiveCheckTimer_Elapsed;
            LiveCheckTimer.Enabled = true;
        }

        private static async void LiveCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bot.Log("Checking live Twitch channels");

            var c = ConfigManager.GetTwitchMonitoredChannels();
            if (c != null && c.Count > 0)
            {
                var helixResponse = await twitchApi.GetStreamsWithUserLogins(c.Select(ORCO_PUZZONE => ORCO_PUZZONE.twitchChannel).ToArray());

                foreach (HelixStream hstream in helixResponse.Data)
                {
                    Bot.OnStreamerLive(hstream.UserName);
                }
            }
        }
    }
}