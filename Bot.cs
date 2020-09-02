using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Splash.Configs;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Splash
{
    class Bot
    {
        public static DiscordClient discord;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        public delegate void StreamerLiveEventHandler(string twitchChannel);
        public static event StreamerLiveEventHandler StreamerLive;

        public enum SplashLogLevel
        {
            Info,
            Warning,
            Error
        }

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Log("Initializing..");

            ConfigManager.Init();
            var token = ConfigManager.GetDiscordToken();

            if (token != string.Empty)
            {
                discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = token,
                    TokenType = TokenType.Bot,

                    AutoReconnect = true,
                    UseInternalLogHandler = true,
                    LogLevel = DSharpPlus.LogLevel.Debug
                });
            }
            else
            {
                return;
            }

            discord.Ready += Discord_Ready;

            /*
             *  Commands
             */
            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "--"
            });
            commands.RegisterCommands<Commands>();

            commands.CommandExecuted += Commands_CommandExecuted;
            commands.CommandErrored += Commands_CommandErrored;

            /*
             *  Message Handling
             */
            discord.MessageCreated += async e =>
            {
                if (e.Message.MentionedUsers.Contains(discord.CurrentUser))
                {
                    await e.Message.RespondAsync(e.Message.Author.Mention);
                }
            };

            /*
             *  Interactivity
             */
            interactivity = discord.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = TimeoutBehaviour.Ignore,
                PaginationTimeout = TimeSpan.FromMinutes(5),
                Timeout = TimeSpan.FromMinutes(2)
            });

            /*
             *  Twitch
             */
            await TwitchManager.Init();
            StreamerLive += Bot_StreamerLive;

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }


        private static Task Discord_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info,
                "Splash",
                $"Client ready",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Error,
                "Splash",
                $"Command error, {e.Exception.GetType()}: {e.Exception.Message}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(DSharpPlus.LogLevel.Info,
                "Splash",
                $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' [{e.Context.Message.Content}]",
                DateTime.Now);

            return Task.CompletedTask;
        }

        public static void OnStreamerLive(string twitchChannel)
        {
            StreamerLive?.Invoke(twitchChannel);
        }
        private static void Bot_StreamerLive(string twitchChannel)
        {
            Log($"Streamer live event triggered for '{twitchChannel}'..", newLine: false);

            var monitoredChannels = ConfigManager.GetTwitchMonitoredChannels();

            Log($"found {monitoredChannels.Select(d => d.ChannelID).Distinct().Count()} discord channels matches", header: false);

            foreach (MonitoredChannel mc in monitoredChannels)
            {
                if (mc.twitchChannel.ToLower() == twitchChannel.ToLower())
                {
                    discord.GetGuildAsync(mc.GuildID).Result.GetChannel(mc.ChannelID).SendMessageAsync($"{twitchChannel} è live!");
                }
            }
        }

        public static void Log(string message, SplashLogLevel logLevel = SplashLogLevel.Info, [CallerMemberName] string callerName = "", bool newLine = true, bool header = true)
        {
            var date = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss K]")}";
            callerName = $"[{callerName}]";

            var str_header = header ? $"{date} {callerName}" : "";
            ConsoleColor cc = new ConsoleColor();

            switch (logLevel)
            {
                case SplashLogLevel.Info:
                    cc = ConsoleColor.Cyan;
                    str_header += header ? " [Info]" : "";
                    break;
                case SplashLogLevel.Warning:
                    cc = ConsoleColor.Yellow;
                    str_header += header ? " [Warning]" : "";
                    break;
                case SplashLogLevel.Error:
                    cc = ConsoleColor.Red;
                    str_header += header ? " [Error]" : "";

                    break;
                default:
                    break;
            }
            Console.ForegroundColor = cc;
            Console.Write($"{str_header} {message}");
            Console.ResetColor();
            Console.Write(newLine ? Environment.NewLine : "");
        }
    }
}