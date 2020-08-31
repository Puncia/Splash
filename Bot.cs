using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;

namespace Splash
{
    class Bot
    {
        static DiscordClient discord;
        static CommandsNextModule commands;
        static InteractivityModule interactivity;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            ConfigParser.Init();
            var token = ConfigParser.GetDiscordToken();

            if (token != string.Empty)
            {
                discord = new DiscordClient(new DiscordConfiguration
                {
                    Token = token,
                    TokenType = TokenType.Bot,

                    AutoReconnect = true,
                    UseInternalLogHandler = true,
                    LogLevel = LogLevel.Debug
                });
            }
            else
                return;

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
                    await e.Message.RespondAsync(e.Message.Author.Mention);
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

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Discord_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info,
                "Splash",
                $"Client ready",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error,
                "Splash",
                $"Command error, {e.Exception.GetType()}: {e.Exception.Message}",
                DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info,
                "Splash",
                $"{e.Context.User.Username} executed '{e.Command.QualifiedName}' [{e.Context.Message.Content}]",
                DateTime.Now);

            return Task.CompletedTask;
        }
    }
}
