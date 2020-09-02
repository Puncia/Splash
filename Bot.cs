using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
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

        public delegate void StreamerLiveEventHandler(string channel);
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

        public static void OnStreamerLive(string stream)
        {
            StreamerLive?.Invoke(stream);
        }
        private static void Bot_StreamerLive(string channel)
        {
            var monitoredChannels = ConfigManager.GetTwitchMonitoredChannels();

            if (monitoredChannels != null)
            {
                foreach (KeyValuePair<string, ulong> c in monitoredChannels)
                {
                    if (c.Key == channel)
                    {
                        //TODO: fix this with custom channel names
                        discord.GetGuildAsync(c.Value).Result.Channels.First(c => c.Name == "twitch").SendMessageAsync($"{channel} è live!");
                    }
                }
            }
        }

        public static void Log(string message, [CallerMemberName] string callerName = "")
        {
            Log(message, SplashLogLevel.Info, callerName);
        }

        public static void Log(string message, SplashLogLevel logLevel, [CallerMemberName] string callerName = "")
        {
            var date = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss K]")}";
            callerName = $"[{callerName}]";
            switch (logLevel)
            {
                case SplashLogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{date} {callerName} [Info]");
                    Console.ResetColor();
                    break;
                case SplashLogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{date} {callerName} [Warning]");
                    Console.ResetColor();
                    break;
                case SplashLogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{date} {callerName} [Error]");
                    Console.ResetColor();
                    break;
                default:
                    break;
            }
        }
    }
}