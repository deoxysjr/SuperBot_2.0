using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SuperBot_2_0.Services;
using SuperBotDLL1_0.RankingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SuperBot_2_0.Modules.Ranks
{
    public class Rank : ModuleBase
    {
        private DiscordShardedClient client = Program._client;

        [Group("rank"), Name("Rank")]
        public class RankGroup : ModuleBase
        {
            [Command("")]
            public async Task GetRank(IUser user = null)
            {
                Ranking.GetRank(Context, user);
                await Task.Delay(1);
            }

            [Command("setbackground"), Alias("setbg")]
            public async Task SetBackground(int back)
            {
                UserInfo info = new UserInfo(Context.User.Id);
                if (!Ranking.BackGroundList.ContainsKey(back))
                    await ReplyAsync("i don't know that background, try another one");
                else
                {
                    if (info.BackGround == back)
                    {
                        await ReplyAsync("You're already using this background");
                    }
                    else
                    {
                        List<ulong> users = new List<ulong>(3) { 245140333330038785, 256164532781580289, 210515625850830848 };
                        if (back == 1 && users.Contains(Context.User.Id))
                        {
                            info.ChangeBackground(back);
                            await Context.Channel.SendFileAsync(Ranking.BackGroundList[back], "this is your background now");
                        }
                        else if (back != 1)
                        {
                            info.ChangeBackground(back);
                            await Context.Channel.SendFileAsync(Ranking.BackGroundList[back], "this is your background now");
                        }
                        else
                        {
                            await ReplyAsync("you can't use background 1");
                        }
                    }
                }
            }

            [Command("setbackground"), Alias("setbg")]
            public async Task SetBackgroundString(string back)
            {
                UserInfo info = new UserInfo(Context.User.Id);
                if (!Ranking.BackGroundStringlist.ContainsKey(back))
                    await ReplyAsync("there is no background with the name " + back);
                else
                {
                    if (info.BackGround == Ranking.BackGroundStringlist[back])
                    {
                        await ReplyAsync("You're already using this background");
                    }
                    else
                    {
                        List<ulong> users = new List<ulong>(3) { 245140333330038785, 256164532781580289, 210515625850830848 };
                        if (back.ToLower() == "shrek" && users.Contains(Context.User.Id))
                        {
                            info.ChangeBackground(Ranking.BackGroundStringlist[back]);
                            await Context.Channel.SendFileAsync(Ranking.BackGroundList[Ranking.BackGroundStringlist[back]], "this is your background now");
                        }
                        else if (back != "shrek")
                        {
                            info.ChangeBackground(Ranking.BackGroundStringlist[back]);
                            await Context.Channel.SendFileAsync(Ranking.BackGroundList[Ranking.BackGroundStringlist[back]], "this is your background now");
                        }
                        else
                        {
                            await ReplyAsync("you can't use background shrek");
                        }
                    }
                }
            }

            [Command("settype"), Alias("sett")]
            public async Task SetRtype(int type)
            {
                UserInfo info = new UserInfo(Context.User.Id);
                if (type == 0)
                {
                    if (info.Ranktype == "embed")
                        await ReplyAsync("It's already set to embed");
                    else
                    {
                        info.ChangeRankType("embed");
                        await ReplyAsync("Your type has been changed to embed");
                    }
                }
                else if (type == 1)
                {
                    if (info.Ranktype == "image")
                        await ReplyAsync("It's already set to image");
                    else
                    {
                        info.ChangeRankType("image");
                        await ReplyAsync("Your type has been changed to image");
                    }
                }
                else
                {
                    await ReplyAsync("I don't know that type");
                }
            }

            [Command("settype"), Alias("sett")]
            public async Task SetRtypeString(string type)
            {
                UserInfo info = new UserInfo(Context.User.Id);
                if (type == "embed")
                {
                    if (info.Ranktype == "embed")
                        await ReplyAsync("It's already set to embed");
                    else
                    {
                        info.ChangeRankType("embed");
                        await ReplyAsync("Your type has been changed to embed");
                    }
                }
                else if (type == "image")
                {
                    if (info.Ranktype == "image")
                        await ReplyAsync("It's already set to image");
                    else
                    {
                        info.ChangeRankType("image");
                        await ReplyAsync("Your type has been changed to image");
                    }
                }
                else
                {
                    await ReplyAsync("I don't know that type");
                }
            }

            [Command("getbackground"), Alias("getbg")]
            public async Task GetBackground()
            {
                UserInfo info = new UserInfo(Context.User.Id);
                await Context.Channel.SendFileAsync(Ranking.BackGroundList[info.BackGround], "This is your back ground");
            }

            [Command("gettype"), Alias("gett")]
            public async Task GetRtype()
            {
                UserInfo info = new UserInfo(Context.User.Id);
                await ReplyAsync("Your rank type is " + info.Ranktype);
            }

            [Command("getbackgroundlist"), Alias("getbgl")]
            public async Task GetBackgroundList()
            {
                var list = new List<string>();
                list.Add("```");
                foreach (var bg in Ranking.BackGroundStringlist)
                {
                    list.Add($"Name: {bg.Key}, Num: {bg.Value}");
                }
                list.Add("```");
                await ReplyAsync(string.Join("\n", list));
            }

            [Command("gettypelist"), Alias("gettl")]
            public async Task GetRtypeList()
            {
                await ReplyAsync("Name: embed, Num: 0\nName: image, Num: 1");
                await Task.Delay(1);
            }
        }

        [Command("leaderboard"), Alias("lb")]
        public async Task LeaderBoard()
        {
            try
            {
                string[] golb = RankUtils.GloLeaderBoard();
                IUser first = client.GetUser(ulong.Parse(golb[0]));
                IUser second = client.GetUser(ulong.Parse(golb[3]));
                IUser third = client.GetUser(ulong.Parse(golb[6]));
                EmbedBuilder builder = new EmbedBuilder();
                builder.AddField("global Leaderboard", $"#1 {first.Username} lvl: {golb[1]} p: {golb[2]}");
                //    + $"\n#2 {second.Id} lvl: {golb[4]} p: {golb[5]}");
                //    + $"\n#3 {third.Username} lvl: {golb[7]} p: {golb[8]}");
                //string[] lolb = RankUtils.LocLeaderBoard(Context, client);
                //IUser lofirst = client.GetUser(ulong.Parse(lolb[0]));
                //IUser losecond = client.GetUser(ulong.Parse(lolb[2]));
                //IUser lothird = client.GetUser(ulong.Parse(lolb[4]));
                //builder.AddField("global Leaderboard", $"#1  {lofirst.Mention} lvl: {lolb[1]}" + $"\n#2 {losecond.Mention} lvl: {lolb[3]}" + $"\n#3 {lothird.Mention} lvl: {lolb[5]}");
                await ReplyAsync("", false, builder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Command("daily")]
        public async Task Daily()
        {
            LevelUser user = new LevelUser();
            user.Load(Context.User.Id);
            await ReplyAsync(user.Daily());
            user.Save(Context.User.Id);
        }

        [Command("resetdaily")]
        public async Task Reset()
        {
            if (Context.User.Id == 245140333330038785)
            {
                //RankUtils.ResetDaily();
                //await ReplyAsync("all dailys have been reset");
            }
            else
            {
                await ReplyAsync("Sorry but only the bot owner can use this command");
            }
        }

        [Command("mine")]
        public async Task Mine(int amount = 1)
        {
            if (amount >= 0 && amount <= 10)
            {
                EmbedBuilder embed = new EmbedBuilder
                {
                    Color = Color.Green
                };
                MineUser user = new MineUser();
                user.Load(Context.User.Id);
                Random rand = new Random();
                string path = Program.levelpath + Context.User.Id + ".xml";
                user.GainXpMine(embed, Context.User);
                user.Save(Context.User.Id);
                await ReplyAsync("", false, Ranking.Miner(embed, path, Context.User.Id.ToString(), amount, Program.mineinv).Build());
                if (File.Exists($"./{Context.User.Id}.png"))
                    File.Delete($"./{Context.User.Id}.png");
            }
            else
                await ReplyAsync("Sorry but I can't pick that much stuf");
        }

        [Command("pick")]
        public async Task Pick(int amount = 1)
        {
            if (amount >= 1 && amount <= 100)
            {
                EmbedBuilder embed = new EmbedBuilder
                {
                    Color = Color.Green
                };
                PickUser user = new PickUser();
                user.Load(Context.User.Id);
                Random rand = new Random();
                string path = Program.levelpath + Context.User.Id + ".xml";
                user.GainXpPick(embed, Context.User);
                user.Save(Context.User.Id);
                await ReplyAsync("", false, Ranking.Picker(embed, path, Context.User.Id.ToString(), amount, Program.baginv).Build());
            }
            else
                await ReplyAsync("Sorry, but I can't pick that much stuf");
        }

        [Command("bag"), Alias("inventory")]
        public async Task Bag(string type = "types")
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Program.levelpath + Context.User.Id + ".xml");
            XmlNode node = doc.SelectSingleNode($"user/ID{Context.User.Id}");
            EmbedBuilder builder = Ranking.Bag(node, type);
            await ReplyAsync("", false, builder.Build());
        }

        [Command("give")]
        public async Task Give(IUser user, double amount)
        {
            if (Context.User == user)
                await ReplyAsync("You have it already");
            else
            {
                LevelUser user1 = new LevelUser();
                LevelUser user2 = new LevelUser();
                user1.Load(Context.User.Id);
                user2.Load(user.Id);
                var UserList = new List<IUser>
                {
                    await Context.Client.GetUserAsync(user1.DcUserId),
                    await Context.Client.GetUserAsync(user2.DcUserId)
                };
                await ReplyAsync("", false, user1.Give(user2, amount, UserList).Build());
                user1.Save(user1.DcUserId);
                user2.Save(user2.DcUserId);
            }
        }

        [Command("prestige")]
        public async Task Prestige()
        {
            LevelUser user = new LevelUser();
            user.Load(Context.User.Id);
            await ReplyAsync(user.PrestigeUser());
        }
    }
}