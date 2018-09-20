using System.Xml;

namespace SuperBot_2._0.Services
{
    internal class CommandUsed
    {
        private static readonly string path = "./file/commandsused.xml";

        public static void CommandAdd()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            string number = xDoc.SelectSingleNode("root/commands").InnerText;
            xDoc.SelectSingleNode("root/commands").InnerText = (int.Parse(number) + 1).ToString();
            xDoc.Save(path);
        }

        public static void ClearAdd(int v)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            string number = xDoc.SelectSingleNode("root/clear").InnerText;
            xDoc.SelectSingleNode("root/clear").InnerText = (int.Parse(number) + v).ToString();
            xDoc.Save(path);
        }

        public static void TotalXpAdd(int xp)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            string number = xDoc.SelectSingleNode("root/totalxp").InnerText;
            xDoc.SelectSingleNode("root/totalxp").InnerText = (ulong.Parse(number) + ulong.Parse(xp.ToString())).ToString();
            xDoc.Save(path);
        }

        public static void GainedMessagesAdd()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            string number = xDoc.SelectSingleNode("root/messages").InnerText;
            xDoc.SelectSingleNode("root/messages").InnerText = (ulong.Parse(number) + 1).ToString();
            xDoc.Save(path);
        }

        public static void TotalDamageAdd(double damage)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            double number = double.Parse(xDoc.SelectSingleNode("root/totaldamage").InnerText);
            xDoc.SelectSingleNode("root/totaldamage").InnerText = (number + damage).ToString();
            xDoc.Save(path);
        }
    }
}