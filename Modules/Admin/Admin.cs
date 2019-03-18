using Discord;
using Discord.Commands;
using SuperBotDLL1_0;
using SuperBotDLL1_0.Classes.GuildUntils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SuperBot_2_0.Modules.Admin
{
    [Name("Admin"), RequireUserPermission(GuildPermission.Administrator)]
    public class Admin : ModuleBase
    {
        private static readonly List<float> AvailableCPU = new List<float>();
        private static readonly List<float> AvailableRAM = new List<float>();
        protected static PerformanceCounter cpuCounter;
        protected static PerformanceCounter ramCounter;
        private static readonly List<PerformanceCounter> CpuCounters = new List<PerformanceCounter>();
        private static readonly List<PerformanceCounter> Core = new List<PerformanceCounter>();

        [Command("clear"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermission(GuildPermission.ManageMessages), RequireContext(ContextType.Guild)]
        [Alias("clr")]
        public async Task Deleteasync(string count = null)
        {
            try
            {
                if (count == null)
                {
                    List<IMessage> messageList = await Context.Channel.GetMessagesAsync().Flatten().ToList();
                    int num = messageList.Count();
                    ITextChannel channel = Context.Channel as ITextChannel;
                    await channel.DeleteMessagesAsync(messageList);
                    Services.CommandUsed.ClearAdd(num + 1);
                    var message = await ReplyAsync($"Deleted the last {num} messages.");
                    _ = await Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => message.DeleteAsync());
                }
                else if (int.Parse(count) < 101)
                {
                    List<IMessage> messageList = await Context.Channel.GetMessagesAsync(int.Parse(count)).Flatten().ToList();
                    int num = messageList.Count();
                    ITextChannel channel = Context.Channel as ITextChannel;
                    await channel.DeleteMessagesAsync(messageList);
                    Services.CommandUsed.ClearAdd(num + 1);
                    var message = await ReplyAsync($"Deleted the last {num} messages.");
                    _ = await Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => message.DeleteAsync());
                }
                else
                {
                    await ReplyAsync("Sorry, but 100 is the maximum");
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
            await ReplyAsync("WIP");
        }

        [Command("command add")]
        public async Task AddCommand(string name, [Remainder]string outputtext)
        {
            try
            {
                List<string> Commands = new List<string>();
                SQLConnection con = new SQLConnection();
                con.ExecuteCommand($"SELECT guildid,name FROM customcommands WHERE guildid = {Context.Guild.Id}");

                if (con.Reader.HasRows)
                {
                    while (con.Reader.Read())
                    {
                        string Name = con.Reader.GetString("name");
                        Commands.Add(Name);
                    }
                }

                if (!Commands.Contains(name))
                {
                    con.ExecuteCommand($"INSERT INTO customcommands (commandid, guildid, name, output) VALUES (NULL, {Context.Guild.Id}, '{name}', '{outputtext}')");
                    await ReplyAsync($"{name} has been added");
                }
                else
                    await ReplyAsync("That command already exists");
                con.CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Command("command remove")]
        public async Task RemoveCommand(string name)
        {
            List<string> Commands = new List<string>();
            SQLConnection con = new SQLConnection();
            con.ExecuteCommand($"SELECT guildid,name FROM customcommands WHERE guildid = {Context.Guild.Id}");

            if (con.Reader.HasRows)
            {
                while (con.Reader.Read())
                {
                    string Name = con.Reader.GetString("name");
                    Commands.Add(Name);
                }
            }

            if (Commands.Contains(name))
            {
                con.ExecuteCommand($"DELETE FROM customcommands WHERE customcommands.name = '{name}'");
                await ReplyAsync($"{name} has been removed");
            }
            else
                await ReplyAsync("That command doesn't exist");
        }

        [Command("addchannel"), RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("removechannel"), RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("disablecmd"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task DisGuild()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableCommands();
                await ReplyAsync("Commands are now enabled in all channels in this guild");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("enablecmd"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task EnGuild()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableCommands();
                await ReplyAsync("Commands are now disabled in specific channels in this guild");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("channellist"), RequireUserPermission(GuildPermission.Administrator)]
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
            builder.Title = "This is the list of disabled channels";
            builder.Description = string.Join("\n", list);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("serverroles")]
        public async Task ServerRoles(int page = 1)
        {
            EmbedBuilder builder = new EmbedBuilder();

            var list = new List<IRole>();
            foreach (IRole role in Context.Guild.Roles)
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
                builder.AddField("Sorry", "Sorry,There aren't that many pages");
            }
            await ReplyAsync("", false, builder.Build());
        }

        [Command("addrole"), RequireUserPermission(GuildPermission.Administrator)]
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
                    await ReplyAsync("Please use an id of a role in this server");
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("removerole"), RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("enableAR"), RequireUserPermission(GuildPermission.Administrator), RequireBotPermission(GuildPermission.ManageRoles)]
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

        [Command("disableAR"), RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("Autorolelist"), RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("enablelevelmessage")]
        public async Task EnableLevelMessages()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableLevelMessage();
                await ReplyAsync("Level messages are now enabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("disablelevelmessage")]
        public async Task DisableLevelMessages()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableLevelMessage();
                await ReplyAsync("Level messages are now disabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("enabledeletemessage")]
        public async Task EnableDeleteLevelMessages()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableDeleteMessage();
                await ReplyAsync("Level message deletion is now enabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("disabledeletemessage")]
        public async Task DisableDeleteLevelMessages()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableLevelMessage();
                await ReplyAsync("Level message deletion is now disabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("enablegainxp")]
        public async Task EnableXpGain()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.EnableXpGain();
                await ReplyAsync("Xp gain is now enabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("disablegainxp")]
        public async Task DisableXpGain()
        {
            try
            {
                GuildChannel guild = new GuildChannel(Context.Guild);
                guild.DisableXpGain();
                await ReplyAsync("Xp gain is now disabled in this server");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
    }
}