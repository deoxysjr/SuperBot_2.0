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
                BattleUser user1 = new BattleUser(Context.User.Id);
                BattleUser user2 = new BattleUser(user.Id);
                BattleInfo Info = new BattleInfo();

                while (user1.Healt > 0.0 && user2.Healt > 0.0)
                {
                    int turn = rand.Next(2);
                    if (turn == 0)
                    {
                        int damage = rand.Next(int.Parse(user1.Damage[0]), int.Parse(user1.Damage[1]));
                        double dealdamage = double.Parse(damage.ToString()) * user1.DamageMultiplier;
                        user2.Healt -= dealdamage;
                        Info.AddDamage(dealdamage);
                    }
                    else if (turn == 1)
                    {
                        int damage = rand.Next(int.Parse(user2.Damage[0]), int.Parse(user2.Damage[1]));
                        double dealdamage = double.Parse(damage.ToString()) * user2.DamageMultiplier;
                        user1.Healt -= dealdamage;
                        Info.AddDamage(dealdamage);
                    }
                    Info.Addturn();
                }
                if (user1.Healt > 0.0)
                {
                    builder.AddField("Victory", $"{Context.User.Username} has won this match with {user1.Healt} health left");
                    //user1.AddWin();
                    //user2.AddLoss();
                }
                else
                {
                    builder.AddField("Victory", $"{user.Username} has won this match with {user2.Healt} health left");
                    //user1.AddLoss();
                    //user2.AddWin();
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