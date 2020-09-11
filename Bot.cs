using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Splash.Configs;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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

        static Mutex logMutex;

        public enum LogLevel
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
            logMutex = new Mutex();
            Log("Initializing..", NewLine: false);

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
                    LogLevel = DSharpPlus.LogLevel.Error
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
                    Bot.Log($"{e.Author.Username}#{e.Author.Discriminator} mentioned me");
                    await e.Message.RespondAsync(e.Author.Mention);
                }
                
                //delete message if it's in #role-assignment
                if (e.Channel.Name == "role-assignment" || e.Channel.Name == "welcome")
                {
                    Bot.Log($"Deleting message in #{e.Channel.Name}: [{e.Author.Username}#{e.Author.Discriminator}] {e.Message.Content}");
                    await e.Message.DeleteAsync();
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
            Log("ready", Header: false);

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
            //Log($"Streamer live event triggered for '{twitchChannel}'..", newLine: false);

            //var monitoredChannels = ConfigManager.GetTwitchMonitoredChannels();

            //Log($"found {monitoredChannels.Select(d => d.ChannelID).Distinct().Count()} discord channels matches", header: false);

            //foreach (MonitoredChannel mc in monitoredChannels)
            //{
            //    if (mc.TwitchChannel.ToLower() == twitchChannel.ToLower())
            //    {
            //        discord.GetGuildAsync(mc.GuildID).Result.GetChannel(mc.ChannelID).SendMessageAsync($"{twitchChannel} è live!");
            //    }
            //}
        }

        public static void Log(string Message, LogLevel LogLevel = LogLevel.Info, [CallerMemberName] string CallerName = "",  bool NewLine = true, bool Header = true)
        {
            logMutex.WaitOne();
            var date = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss K]")}";
            CallerName = $"[{CallerName}]";

            var str_header = Header ? $"{date} {CallerName}" : "";
            ConsoleColor cc = new ConsoleColor();

            switch (LogLevel)
            {
                case LogLevel.Info:
                    cc = ConsoleColor.White;
                    str_header += Header ? " [Info] " : "";
                    break;
                case LogLevel.Warning:
                    cc = ConsoleColor.Yellow;
                    str_header += Header ? " [Warning] " : "";
                    break;
                case LogLevel.Error:
                    cc = ConsoleColor.Red;
                    str_header += Header ? " [Error] " : "";

                    break;
                default:
                    break;
            }
            Console.ForegroundColor = cc;
            Console.Write($"{str_header}{Message}");
            Console.ResetColor();
            Console.Write(NewLine ? Environment.NewLine : "");
            logMutex.ReleaseMutex();
        }
    }
}