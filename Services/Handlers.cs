using Discord.Commands;
using Discord.WebSocket;
using SuperBot_2_0;
using SuperBotDLL1_0.Classes.GuildUntils;
using SuperBotDLL1_0.Gambling;
using SuperBotDLL1_0.RankingSystem;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SuperBot_2._0.Services
{
    internal class Handlers
    {
        private static readonly IServiceProvider services = Program._services;
        private static readonly DiscordSocketClient client = Program._client;
        private static readonly CommandService commands = Program._commands;

        public static async Task InitHandlers()
        {
            client.MessageReceived += HandleCommandAsync;
            client.UserJoined += HandleUserJoin;
            client.UserLeft += HandleUserLeft;
            client.Ready += async () => await client.SetGameAsync($"Guild users {client.Guilds.Sum(x => x.MemberCount)}");
            await Program._commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private static async Task HandleUserLeft(SocketGuildUser arg)
        {
            await Task.Delay(1);
        }

        private static async Task HandleUserJoin(SocketGuildUser arg)
        {
            GuildChannel guild = new GuildChannel(arg.Guild);
            if (guild.AutoRoleOn)
            {
                if(guild.RoleList[0] != 0)
                    await arg.AddRoleAsync(arg.Guild.GetRole(guild.RoleList[0]));
            }
        }

        private static async Task HandleCommandAsync(SocketMessage arg)
        {
            string userpath = Program.levelpath + arg.Author.Id + ".xml";
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot) return;
            int pos = 0;

            LevelUser user = new LevelUser();
            if (!File.Exists(userpath))
                Console.WriteLine(user.AddNewUserRank(arg.Author.Id.ToString()));
            Ranking.CheckUser(userpath, arg.Author.Id.ToString(), Program.mineinv, Program.baginv, Program.craftlist);

            UserInfo info = new UserInfo(arg.Author.Id);
            info.AddMessage();

            var context = new SocketCommandContext(client, msg);
            if (msg.HasStringPrefix("%", ref pos) || msg.HasMentionPrefix(client.CurrentUser, ref pos))
            {
                GuildChannel guild = new GuildChannel(context.Guild);
                if (!guild.CommandsOn || guild.DisChannelsList.Contains(arg.Channel.Id) == false || msg.Content == "%disable")
                {
                    var result = await commands.ExecuteAsync(context, pos, services);

                    if (!result.IsSuccess)
                        Utils.CustomErrors(msg, result, context);
                    else if (result.IsSuccess)
                    {
                        info.AddCommand();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{DateTime.Now,-19} [{msg.Channel.Name}] [{msg.Author.Id}] Used {msg.ToString()}");
                        Console.ForegroundColor = ConsoleColor.White;
                        CommandUsed.CommandAdd();
                    }
                }
                else
                {
                    var message = await context.Channel.GetMessageAsync(msg.Id);
                    await message.DeleteAsync();
                    CommandUsed.ClearAdd(1);
                    //await msg.Channel.SendMessageAsync("commands can't be used in this channel");
                }
            }

            Hangman.GetInput(msg.Content.ToLower(), context);

            if (arg.Author.IsBot && arg.Author.Id != 372615866652557312)
                return;
            else
            {
                try
                {
                    CommandUsed.GainedMessagesAdd();
                    user.Load(arg.Author.Id);
                    int xp = new Random().Next(1, 5);
                    CommandUsed.TotalXpAdd(xp);
                    user.GainXpUser(arg, xp);
                    if (File.Exists($"./userimg.png"))
                        File.Delete($"./userimg.png");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"User:{arg.Author.Id}" + ex.ToString());
                }
            }
        }
    }
}