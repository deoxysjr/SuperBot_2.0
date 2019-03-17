using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuperBotDLL1_0.Classes.GuildUntils;
using SuperBotDLL1_0.Gambling;
using SuperBotDLL1_0.RankingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SuperBot_2_0.Services
{
    internal class Handlers
    {
        private IServiceProvider Services { get; }
        private DiscordShardedClient Client { get; }
        private CommandService Commands { get; }
        private List<DiscordSocketClient> Shards { get; }
        private int TotalUsers = 0;

        public Handlers(IServiceProvider service, DiscordShardedClient client, CommandService commands)
        {
            Services = service;
            Client = client;
            Commands = commands;

            Shards = Client.Shards.ToList();

            Client.MessageReceived += HandleCommandAsync;
            Client.UserJoined += HandleUserJoin;
            Client.UserLeft += HandleUserLeft;
            Client.ShardReady += ShardReady;
            Client.Log += Logger;
            Commands.Log += Logger;

            commands.CommandExecuted += CommandExecuted;
        }

        public async Task InitializeAsync()
        {
            await Program._commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            try
            {
                string userpath = Program.levelpath + arg.Author.Id + ".xml";
                if (!(arg is SocketUserMessage msg)) return;

                if (msg.Author.Id == Client.CurrentUser.Id || msg.Author.IsBot) return;
                int pos = 0;

                LevelUser user = new LevelUser();
                if (!File.Exists(userpath))
                    Console.WriteLine(user.AddNewUserRank(arg.Author.Id.ToString()));

                UserInfo info = new UserInfo(arg.Author.Id);
                info.AddMessage();

                var context = new ShardedCommandContext(Client, msg);
                GuildChannel guild = new GuildChannel(context.Guild);
                if (msg.HasStringPrefix("%%", ref pos) || msg.HasMentionPrefix(Client.CurrentUser, ref pos))
                {
                    if (!guild.CommandsOn || guild.DisChannelsList.Contains(arg.Channel.Id) == false || msg.Content == "%disable")
                    {
                        var result = await Commands.ExecuteAsync(context, pos, Services);

                        if (!result.IsSuccess)
                            Utils.CustomErrors(msg, result, context);
                        else if (result.IsSuccess)
                        {
                            info.AddCommand();
                            CommandUsed.CommandAdd();
                        }
                    }
                    else
                    {
                        var message = await context.Channel.GetMessageAsync(msg.Id);
                        await message.DeleteAsync();
                        CommandUsed.ClearAdd(1);
                    }
                }

                if (arg.Author.IsBot && arg.Author.Id != 372615866652557312 || !guild.GainXp)
                    return;
                else
                {
                    try
                    {
                        CommandUsed.GainedMessagesAdd();
                        user.Load(arg.Author.Id);
                        int xp = new Random().Next(1, 5);
                        CommandUsed.TotalXpAdd(xp);
                        user.GainXpUser(arg, guild, xp);
                        if (File.Exists($"./userimg.png"))
                            File.Delete($"./userimg.png");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"User:{arg.Author.Id}" + ex.ToString());
                    }
                }

                Hangman.GetInput(msg.Content.ToLower(), context);
            }
            catch (Exception)
            {
            }
        }

        private static Task Logger(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message} {message.Exception}");
                    Console.ResetColor();
                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message}");
                    Console.ResetColor();
                    break;

                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message}");
                    Console.ResetColor();
                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message}");
                    Console.ResetColor();
                    break;
            }
            Console.ForegroundColor = ConsoleColor.White;

            return Task.CompletedTask;
        }

        private async Task CommandExecuted(Optional<CommandInfo> arg1, ICommandContext context, IResult arg3)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now,-19} [{context.Channel.Name}] [Shard: #{Client.GetShardIdFor(context.Guild)}] [{context.User.Id}] Used {context.Message.Content}");
            Console.ForegroundColor = ConsoleColor.White;
            await Task.Delay(1);
        }

        private async Task ShardReady(DiscordSocketClient arg)
        {
            foreach (var guild in arg.Guilds)
                TotalUsers += guild.Users.Count;

            await Client.SetGameAsync($"Guild users {TotalUsers}", null, ActivityType.Streaming);
        }

        private async Task AddUsersAsync()
        {
            await Task.Delay(1);
            await Client.SetGameAsync($"Guild users {TotalUsers}", null, ActivityType.Streaming);
        }

        private async Task HandleUserLeft(SocketGuildUser arg)
        {
            await Client.SetGameAsync($"Guild users {--TotalUsers}", null, ActivityType.Streaming);
        }

        private async Task HandleUserJoin(SocketGuildUser arg)
        {
            GuildChannel guild = new GuildChannel(arg.Guild);
            if (guild.AutoRoleOn)
            {
                if (guild.RoleList[0] != 0)
                    await arg.AddRoleAsync(arg.Guild.GetRole(guild.RoleList[0]));
            }

            await Client.SetGameAsync($"Guild users {++TotalUsers}", null, ActivityType.Streaming);
        }
    }
}