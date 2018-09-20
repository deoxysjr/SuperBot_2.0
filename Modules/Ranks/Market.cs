using Discord;
using Discord.Commands;
using SuperBot_2_0;
using SuperBotDLL1_0.MarketPlace;
using System;
using System.Threading.Tasks;

namespace SuperBot_2._0.Modules.Ranks
{
    [Group("Market")]
    public class Market : ModuleBase
    {
        [Command("buy")]
        public async Task BuyItem(string item = "items", string amount = null)
        {
            Random rand = new Random();
            int retu = rand.Next(70, 120);
            double num = retu / 100.0;
            await ReplyAsync("Nothing here **go away**!");
        }

        [Command("sell")]
        public async Task SellItems(string item = "items", string amount = null)
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Green
            };
            await ReplyAsync("", false, Marketplace.Sell(builder, Context.User.Id.ToString().ToLower(), item, amount, Program.mineprices, Program.pickprices, Program.craftprice));
        }
    }
}