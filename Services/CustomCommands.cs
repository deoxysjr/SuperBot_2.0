using Discord.Commands;
using SuperBotDLL1_0;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperBot_2_0.Services
{
    public class CustomCommands
    {
        public static async Task<bool> ExecuteCommands(ICommandContext context, int posision)
        {
            if (GetCustomCommands(context.Guild.Id).ContainsKey(context.Message.Content.Remove(0, posision)))
            {
                string cmdoutput = GetCustomCommands(context.Guild.Id)[context.Message.Content.Remove(0, posision)];

                cmdoutput = cmdoutput.Replace("$date", DateTime.Now.ToShortDateString());

                await context.Channel.SendMessageAsync(cmdoutput);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetCustomCommands(ulong guildid)
        {
            Dictionary<string, string> Commands = new Dictionary<string, string>();
            SQLConnection connection = new SQLConnection();
            connection.ExecuteCommand($"SELECT name,output FROM customcommands WHERE guildid = {guildid}");

            if (connection.Reader.HasRows)
            {
                while (connection.Reader.Read())
                {
                    string name = connection.Reader.GetString("name");
                    string output = connection.Reader.GetString("output");
                    Commands.Add(name, output);
                }
            }

            connection.CloseConnection();

            return Commands;
        }
    }
}
