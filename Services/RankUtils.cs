using Discord.Commands;
using Discord.WebSocket;
using SuperBot_2_0;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SuperBot_2._0.Services
{
    internal class RankUtils
    {
        public static string[] GloLeaderBoard
        {
            get
            {
                string first = ""; int firstlvl = 0;
                string second = ""; int secondlvl = 0;
                string third = ""; int thirdlvl = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(Program.levelpath);
                XmlNode root = doc.SelectSingleNode("root/users");
                foreach (XmlNode node in root)
                {
                    if (int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText) > firstlvl && firstlvl == 0)
                    {
                        first = $"{node.Name.Replace("ID", "")}";
                        firstlvl = int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText);
                    }
                    else if (int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText) > firstlvl)
                    {
                        third = second;
                        thirdlvl = secondlvl;
                        second = first;
                        secondlvl = firstlvl;
                        first = $"{node.Name.Replace("ID", "")}";
                        firstlvl = int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText);
                    }
                    else if (int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText) > secondlvl && secondlvl == 0)
                    {
                        second = $"{node.Name.Replace("ID", "")}";
                        secondlvl = int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText);
                    }
                    else if (int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText) > secondlvl)
                    {
                        third = second;
                        thirdlvl = secondlvl;
                        second = $"{node.Name.Replace("ID", "")}";
                        secondlvl = int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText);
                    }
                    else if (int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText) > thirdlvl)
                    {
                        third = $"{node.Name.Replace("ID", "")}";
                        thirdlvl = int.Parse(node.SelectSingleNode("level/currentlvl").Attributes[0].InnerText);
                    }
                }
                string[] lb = { first, firstlvl.ToString(), second, secondlvl.ToString(), third, thirdlvl.ToString() };
                return lb;
            }
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

        public static void ResetDaily()
        {
            foreach (string userpath in Directory.GetFiles("./file/ranks/users"))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(userpath);
                if (doc.GetElementsByTagName("lastdaily").Count > 0)
                {
                    doc.GetElementsByTagName("lastdaily")[0].Attributes["date"].InnerText = "";
                    doc.Save(userpath);
                }
            }
        }
    }
}