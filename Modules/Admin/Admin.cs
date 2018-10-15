using Discord;
using Discord.Commands;
using SuperBot_2_0;
using SuperBotDLL1_0.Classes.GuildUntils;
using SuperBotDLL1_0.RankingSystem;
using SuperBotDLL1_0.Untils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SuperBot_2._0.Modules.Admin
{
    [Name("Admin")]
    public class Admin : ModuleBase
    {
        private static List<float> AvailableCPU = new List<float>();
        private static List<float> AvailableRAM = new List<float>();
        protected static PerformanceCounter cpuCounter;
        protected static PerformanceCounter ramCounter;
        private static List<PerformanceCounter> cpuCounters = new List<PerformanceCounter>();
        private static List<PerformanceCounter> core = new List<PerformanceCounter>();

        [Command("playing"), RequireOwner]
        [Alias("play")]
        public async Task Play(string game)
        {
            if (Context.User.Id == 245140333330038785)
            {
                if (game == "time")
                {
                    var time = $"{DateTime.Now,-19}";
                    await Program._client.SetGameAsync(time);
                }
                else
                    await Program._client.SetGameAsync(game);
            }
            else
            {
                await ReplyAsync("Sorry but only the bot owner can use this command");
            }
        }

        [Command("clear"), RequireBotPermission(GuildPermission.ManageMessages), RequireContext(ContextType.Guild)]
        [Alias("clr")]
        public async Task Deleteasync(string count = null)
        {
            try
            {
                if (count == null)
                {
                    var messageList = await Context.Channel.GetMessagesAsync().Flatten();
                    int num = messageList.Count();
                    await Context.Channel.DeleteMessagesAsync(messageList);
                    await ReplyAsync($"Deleted the last {num} messages.");
                    Services.CommandUsed.ClearAdd(num + 1);
                    var message = await Context.Channel.GetMessagesAsync(1).Flatten();
                    await Task.Delay(1000);
                    await Context.Channel.DeleteMessagesAsync(message);
                }
                else if (int.Parse(count) < 101)
                {
                    var messageList = await Context.Channel.GetMessagesAsync(int.Parse(count)).Flatten();
                    int num = messageList.Count();
                    await Context.Channel.DeleteMessagesAsync(messageList);
                    await ReplyAsync($"Deleted the last {num} messages.");
                    Services.CommandUsed.ClearAdd(num + 1);
                    var message = await Context.Channel.GetMessagesAsync(1).Flatten();
                    await Task.Delay(1000);
                    await Context.Channel.DeleteMessagesAsync(message);
                }
                else
                {
                    await ReplyAsync("sorry but 100 is the maximum");
                }
            }
            catch
            {
                await ReplyAsync("Couldn't delete messages: Insufficient role");
            }
        }

        [Command("invite"), RequireBotPermission(GuildPermission.CreateInstantInvite), RequireUserPermission(GuildPermission.CreateInstantInvite)]
        public async Task CreateInvite()
        {
            await ReplyAsync("https://discord.gg/nZFVvTW");
        }

        [Command("uptime")]
        public async Task UpTime()
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                Title = "Status",
                Color = Color.LightGrey
            };

            var uptime = DateTime.Now - Program.StartupTime;
            builder.AddField("Uptime", $"I'm now up for \n{Other.CalculateTimeWithSeconds((int)Math.Round(uptime.TotalSeconds, 0))}");
            //builder = Other.GetCpuPreformance(builder);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("guild"), RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Users(int days)
        {
            int users = await Context.Guild.PruneUsersAsync(days, true);
            await ReplyAsync($"{users} users have not been online for {days} days");
        }

        [Command("addchannel"), RequireUserPermission(GuildPermission.BanMembers)]
        [Alias("exclude")]
        public async Task AddChannel(IChannel channel)
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.AddChannel(channel.Id);
                await ReplyAsync($"{channel.Name} has been added to the list");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("removechannel"), RequireUserPermission(GuildPermission.BanMembers)]
        public async Task RemChannel(IChannel channel)
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.RemoveChannel(channel.Id);
                await ReplyAsync($"{channel.Name} has been removed from the list");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("disablecmd"), RequireUserPermission(GuildPermission.BanMembers)]
        public async Task DisGuild()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableCommands();
                await ReplyAsync("Commands are now enabled in all channels on this guild");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("enablecmd"), RequireUserPermission(GuildPermission.BanMembers)]
        public async Task EnGuild()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableCommands();
                await ReplyAsync("Commands are now disabled in specific channels on this guild");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("channellist"), RequireUserPermission(GuildPermission.BanMembers)]
        public async Task ListChannels()
        {
            List<string> list = new List<string>();
            EmbedBuilder builder = new EmbedBuilder();
            GuildChannel guild = new GuildChannel(Context.Guild);
            foreach (ulong channelid in guild.DisChannelsList)
            {
                var channel = await Context.Client.GetChannelAsync(channelid);
                list.Add("#" + channel.Name);
            }
            builder.Title = "this is the list of disabled channels";
            builder.Description = string.Join("\n", list);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("serverroles")]
        public async Task ServerRoles(int page = 1)
        {
            EmbedBuilder builder = new EmbedBuilder();
            
            var list = new List<IRole>();
            foreach(IRole role in Context.Guild.Roles)
            {
                if (role.Name != "@everyone")
                    list.Add(role);
            }
            list.Sort();
            if (Math.Ceiling(list.Count / 20.0) >= page)
            {
                var pagelist = new List<string>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (i >= (page - 1) * 20 && i < (page - 1) * 20 + 20)
                    {
                        pagelist.Add($"#{i + 1} Name: {list[i].Name}, Id: {list[i].Id}");
                    }
                }
                builder.AddField("Role list", string.Join("\n", pagelist));
                EmbedFooterBuilder footer = new EmbedFooterBuilder
                {
                    Text = $"page ({page}/{Math.Ceiling(list.Count / 20.0)})"
                };
                builder.Footer = footer;
            }
            else
            {
                builder.AddField("Sorry", "Sorry There aren't that many pages");
            }
            await ReplyAsync("", false, builder.Build());
        }

        [Command("addrole"), RequireOwner]
        public async Task AddRole(ulong role)
        {
            var serverroles = new List<ulong>();
            foreach (IRole Role in Context.Guild.Roles)
                serverroles.Add(Role.Id);
            try
            {
                if (serverroles.Contains(role))
                {
                    GuildChannel guild = new GuildChannel(Context.Guild);
                    guild.AddRole(role);
                    IRole cur = Context.Guild.GetRole(role);
                    await ReplyAsync($"{cur.Name} has been added to the list");
                }
                else
                {
                    await ReplyAsync("please use an id of a role in this server");
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("removerole"), RequireOwner]
        public async Task RemRole(ulong role)
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.RemoveRole(role);
                IRole cur = Context.Guild.GetRole(role);
                await ReplyAsync($"{cur.Name} has been removed from the list");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("enableAR")]
        public async Task Enableautorole()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableAutoRole();
                await ReplyAsync("Autorole is now enabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("disableAR")]
        public async Task Disableautorole()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableAutoRole();
                await ReplyAsync("Autorole is now disabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("Autorolelist")]
        public async Task RoleList()
        {
            EmbedBuilder builder = new EmbedBuilder();
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                if (guild.RoleList[0] == 0)
                {
                    builder.AddField("Error", "There are no autoroles");
                    await ReplyAsync("", false, builder.Build());
                }
                else
                {
                    var list = new List<string>();
                    foreach (ulong role in guild.RoleList)
                    {
                        IRole cur = Context.Guild.GetRole(role);
                        list.Add($"Name: {cur.Name}, Id: {cur.Id}");
                    }
                    builder.AddField("Roles", string.Join(",\n", list));
                    await ReplyAsync("", false, builder.Build());
                }
            }
            catch (Exception ex)
            {
                builder.AddField("Error", ex.ToString());
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("CU"), RequireOwner]
        public async Task CheckUsers()
        {
            try
            {
                string[] users = Directory.GetFileSystemEntries("./file/ranks/users");
                Ranking.CheckUsers(users, Program.mineinv, Program.baginv, Program.craftlist);
                await ReplyAsync("All users are now up dated");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.ToString());
            }
        }
    }
}