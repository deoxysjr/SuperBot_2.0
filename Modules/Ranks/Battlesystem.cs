using Discord;
using Discord.Commands;
using SuperBot_2._0.Services;
using SuperBotDLL1_0.BattleSystem;
using SuperBotDLL1_0.RankingSystem;
using System;
using System.Threading.Tasks;

namespace SuperBot_2._0.Modules.Ranks
{
    [Name("Battle")]
    public class Battlesystem : ModuleBase
    {
        [Command("Battle")]
        public async Task Battle(IUser user)
        {
            try
            {
                //if (Context.User.Id == user.Id)
                //    await ReplyAsync("you can't battle your self");
                EmbedBuilder builder = new EmbedBuilder
                {
                    Color = Color.DarkTeal,
                    Title = $"a battle betwean {Context.User.Username} and {user.Username}"
                };
                Random rand = new Random();
                BattleUser[] users = { new BattleUser(Context.User.Id), new BattleUser(user.Id) };
                UserInfo[] usersinfo = { new UserInfo(Context.User.Id), new UserInfo(user.Id) };
                BattleInfo Info = new BattleInfo();

                while (users[0].Healt > 0.0 && users[1].Healt > 0.0)
                {
                    int turn = rand.Next(2);
                    if (turn == 0)
                    {
                        int damage = rand.Next(4, 7);
                        double dealdamage = (double)damage * users[0].DamageMultiplier;
                        users[1].Healt -= dealdamage;
                        Info.AddDamage(dealdamage);
                    }
                    else if (turn == 1)
                    {
                        int damage = rand.Next(4, 7);
                        double dealdamage = (double)damage * users[1].DamageMultiplier;
                        users[0].Healt -= dealdamage;
                        Info.AddDamage(dealdamage);
                    }
                    Info.Addturn();
                }
                if (users[0].Healt > 0.0)
                {
                    builder.AddField("Victory", $"{Context.User.Username} has won this match with {users[0].Healt} health left");
                    if(Context.User.Id != user.Id)
                    {
                        usersinfo[0].AddWin();
                        usersinfo[1].AddLost();
                    }
                }
                else
                {
                    builder.AddField("Victory", $"{user.Username} has won this match with {users[1].Healt} health left");
                    if (Context.User.Id != user.Id)
                    {
                        usersinfo[0].AddLost();
                        usersinfo[1].AddWin();
                    }
                }
                builder.AddField("Battle info", $"Total damage dealt: {Info.TotalDamage}\nTotal turns taken: {Info.Totalturs}");
                CommandUsed.TotalDamageAdd(Info.TotalDamage);
                await ReplyAsync("", false, builder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        [Command("upgrade")]
        public async Task UpgradeUser(string type = "")
        {
            LevelUser user = new LevelUser();
            user.Load(Context.User.Id);
            await ReplyAsync("", false, BattleSystem.Upgrade(type, user).Build());
            user.Save(Context.User.Id);
        }
    }
}