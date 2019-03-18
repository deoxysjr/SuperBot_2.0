using Discord.Commands;
using Discord.WebSocket;
using SuperBotDLL1_0;
using System;
using System.Collections.Generic;
using System.Xml;

namespace SuperBot_2_0.Services
{
    internal class RankUtils
    {
        public static string[] GloLeaderBoard()
        {
            SQLConnection connection = new SQLConnection();
            connection.ExecuteCommand("SELECT users.dcuserid,users.sbuserid,curxp,curlvl,prestige FROM leveluser INNER JOIN users ON users.sbuserid = leveluser.sbuserid ORDER BY prestige DESC, curlvl DESC, curxp DESC LIMIT 3");
            Console.WriteLine(connection.Reader.HasRows);
            List<string> list = new List<string>();
            int cur = 0;
            if (connection.Reader.HasRows)
            {
                while (connection.Reader.Read())
                {
                    cur++;
                    list.Add(connection.Reader.GetUInt64("dcuserid").ToString());
                    list.Add(connection.Reader.GetInt32("curlvl").ToString());
                    list.Add(connection.Reader.GetInt32("prestige").ToString());
                }
            }
            Console.WriteLine(cur);
            Console.WriteLine(list.Count);
            connection.CloseConnection();
            connection.Dispose();
            return list.ToArray();
        }

        public static string[] LocLeaderBoard(ICommandContext Context, DiscordSocketClient client)
        {
            string first = ""; int firstlvl = 0;
            string second = ""; int secondlvl = 0;
            string third = ""; int thirdlvl = 0;
            XmlDocument doc = new XmlDocument();
            doc.Load(Program.levelpath);
            XmlNode root = doc.SelectSingleNode("root/users");
            var list = new List<ulong>();
            foreach (var guild in client.Guilds)
            {
                if (guild.Id == Context.Guild.Id)
                {
                    foreach (var user in guild.Users)
                    {
                        list.Add(user.Id);
                    }
                    break;
                }
            }

            foreach (ulong user in list)
            {
                if (int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText) > firstlvl && firstlvl == 0)
                {
                    first = $"{user}";
                    firstlvl = int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText);
                }
                else if (int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText) > firstlvl)
                {
                    third = second;
                    thirdlvl = secondlvl;
                    second = first;
                    secondlvl = firstlvl;
                    first = $"{user}";
                    firstlvl = int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText);
                }
                else if (int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText) > secondlvl && secondlvl == 0)
                {
                    second = $"{user}";
                    secondlvl = int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText);
                }
                else if (int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText) > secondlvl)
                {
                    third = second;
                    thirdlvl = secondlvl;
                    second = $"{user}";
                    secondlvl = int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText);
                }
                else if (int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText) > thirdlvl)
                {
                    third = $"{user}";
                    thirdlvl = int.Parse(root.SelectSingleNode($"{user}/level/currentlvl").Attributes[0].InnerText);
                }
            }
            string[] lb = { first, firstlvl.ToString(), second, secondlvl.ToString(), third, thirdlvl.ToString() };
            return lb;
        }
    }
}