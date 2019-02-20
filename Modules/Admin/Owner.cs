using Discord;
using Discord.Commands;
using SuperBot_2_0;
using SuperBotDLL1_0.RankingSystem;
using SuperBotDLL1_0.Untils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SuperBot_2._0.Modules.Admin
{
    [Name("Owner"), RequireOwner]
    public class Owner : ModuleBase
    {
        [Command("restart")]
        public async Task Restartbot()
        {
            await ReplyAsync("Restarting SuperBot");
            await Task.Delay(1000);
            Process.Start(@"D:\Super Bot\SuperBot_2.0\SuperBot_2.0\ReStartSuperbot\bin\Debug\ReStartSuperbot.exe");
        }

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
                await ReplyAsync("Sorry, but only the bot owner can use this command");
            }
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

        [Command("status"), RequireOwner]
        public async Task Status(IUser user = null)
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Blue
            };
            builder = Ranking.GetMineRank(builder, Context, user);
            builder = Ranking.GetPickRank(builder, Context, user);
            builder = Ranking.GetCraftRank(builder, Context, user);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("forcelevelup"), RequireOwner]
        public async Task LevelUpUser(IUser getuser = null)
        {
            if (getuser == null)
            {
                LevelUser user = new LevelUser();
                user.Load(Context.User.Id);
                user.GainXpUser(Context, (user.NeedXp - user.CurrentXp));
                user.Save(Context.User.Id);
                await ReplyAsync("succes!");
            }
            else
            {
                LevelUser user = new LevelUser();
                user.Load(getuser.Id);
                user.GainXpUser(Context, getuser, (user.NeedXp - user.CurrentXp));
                user.Save(getuser.Id);
                await ReplyAsync("succes!");
            }
        }
    }
}