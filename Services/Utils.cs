using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace SuperBot_2_0.Services
{
    internal class Utils
    {
        public static void AddFakeHeaders(HttpClient http)
        {
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1");
            http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
        }

        public static string CurrentTime()
        {
            string time = "";
            int hourchar = 0;
            int minutechar = 0;
            int secondchar = 0;
            int millisecondchar = 0;
            foreach (char c in DateTime.Now.Hour.ToString())
                hourchar++;
            foreach (char c in DateTime.Now.Minute.ToString())
                minutechar++;
            foreach (char c in DateTime.Now.Second.ToString())
                secondchar++;
            foreach (char c in DateTime.Now.Millisecond.ToString())
                millisecondchar++;

            if (hourchar == 1)
                time = time + $"0{DateTime.Now.Hour}:";
            else
                time = time + $"{DateTime.Now.Hour}:";
            if (minutechar == 1)
                time = time + $"0{DateTime.Now.Minute}:";
            else
                time = time + $"{DateTime.Now.Minute}:";
            if (secondchar == 1)
                time = time + $"0{DateTime.Now.Second}:";
            else
                time = time + $"{DateTime.Now.Second}:";
            if (millisecondchar == 1)
                time = time + $"{DateTime.Now.Millisecond}00";
            else if (millisecondchar == 2)
                time = time + $"{DateTime.Now.Millisecond}0";
            else
                time = time + $"{DateTime.Now.Millisecond}";
            return time;
        }

        public static string SizeSuffix(long value, string[] SizeSuffixes, int decimalPlaces = 2)
        {
            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

        public static int getRandInt(int min, int max)
        {
            return new Random().Next(min, max + 1); //for ints
        }

        public static void AddTags(string tagslist, string type)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(@"./tags.xml");
                string procestags = tagslist.Replace(" ", "+");
                string[] tags = procestags.Split('+');
                XmlNode node = doc.SelectSingleNode($"root/{type}");
                bool exists = false;
                foreach (var tag in tags)
                {
                    foreach (XmlNode nod in node.SelectNodes("tag"))
                    {
                        if (nod.InnerText == tag.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists == false)
                    {
                        XmlElement newtag = doc.CreateElement("tag");
                        newtag.InnerText = tag.ToString();
                        node.AppendChild(newtag);
                    }
                    else
                    {
                        exists = false;
                    }
                }
                doc.Save(@"./tags.xml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        public static async Task<string> SearchYandere(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"https://yande.re/post.xml?limit=100&tags={tag}";
            try
            {
                var toReturn = await Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        AddFakeHeaders(http);
                        var data = await http.GetStreamAsync(website).ConfigureAwait(false);
                        var doc = new XmlDocument();
                        doc.Load(data);
                        int amount = 0;
                        foreach (XmlNode nod in doc.DocumentElement)
                        {
                            amount++;
                        }

                        var node = doc.LastChild.ChildNodes[getRandInt(0, amount - 1)];
                        string type = "yandere";
                        string tags = node.Attributes["tags"].Value;
                        AddTags(tags, type);
                        var url = node.Attributes["file_url"].Value;
                        if (!url.StartsWith("http"))
                            url = "https:" + url;
                        return url + "`" + node.Attributes["tags"].Value;
                    }
                }).ConfigureAwait(false);
                return toReturn;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> SearchRule34(string tag)
        {
            tag = tag.Replace(" ", "_");
            string website = $"https://rule34.xxx/index.php?page=dapi&s=post&q=index&tags={tag}";
            try
            {
                var toReturn = await Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        AddFakeHeaders(http);
                        var data = await http.GetStreamAsync(website).ConfigureAwait(false);
                        var doc = new XmlDocument();
                        doc.Load(data);
                        int amount = 0;
                        foreach (XmlNode nod in doc.DocumentElement)
                        {
                            amount++;
                        }
                        var node = doc.LastChild.ChildNodes[getRandInt(0, amount - 1)];

                        string type = "rule34";
                        string tags = node.Attributes["tags"].Value;
                        AddTags(tags, type);
                        var url = node.Attributes["file_url"].Value;
                        if (!url.StartsWith("http"))
                            url = "https:" + url;
                        return url + "`" + node.Attributes["tags"].Value;
                    }
                }).ConfigureAwait(false);
                return toReturn;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async static void CustomErrors(SocketUserMessage msg, IResult result, SocketCommandContext context)
        {
            string message = "";
            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    message = $"I don't know what your talking about what is {msg.ToString()}";
                    break;

                case CommandError.BadArgCount:
                    message = "You don't have the right argument count";
                    break;

                case CommandError.Exception:
                    message = result.ErrorReason;
                    break;

                case CommandError.MultipleMatches:
                    message = "I think I'm on drugs because I see multiple like that";
                    break;

                case CommandError.ObjectNotFound:
                    message = "I don't know where that object is";
                    break;

                case CommandError.ParseFailed:
                    message = "I tried to Parse it but it didn't work";
                    break;

                case CommandError.UnmetPrecondition:
                    message = "there are preconditions for a reason you know";
                    break;

                case CommandError.Unsuccessful:
                    message = "sorry master I tried but I coulded do it";
                    break;
            }
            await context.Channel.SendMessageAsync(message);
        }

        public static string CalculateTimeWithSeconds(int seconds)
        {
            if (seconds == 0)
                return "No time.";

            int years, minutes, months, days, hours = 0;

            minutes = seconds / 60;
            seconds %= 60;
            hours = minutes / 60;
            minutes %= 60;
            days = hours / 24;
            hours %= 24;
            months = days / 30;
            days %= 30;
            years = months / 12;
            months %= 12;

            string animeWatched = "";

            if (years > 0)
            {
                animeWatched += years;
                if (years == 1)
                    animeWatched += " **year**";
                else
                    animeWatched += " **years**";
            }

            if (months > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += months;
                if (months == 1)
                    animeWatched += " **month**";
                else
                    animeWatched += " **months**";
            }

            if (days > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += days;
                if (days == 1)
                    animeWatched += " **day**";
                else
                    animeWatched += " **days**";
            }

            if (hours > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += hours;
                if (hours == 1)
                    animeWatched += " **hour**";
                else
                    animeWatched += " **hours**";
            }

            if (minutes > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += minutes;
                if (minutes == 1)
                    animeWatched += " **minute**";
                else
                    animeWatched += " **minutes**";
            }

            if (seconds > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += " and ";
                animeWatched += seconds;
                if (seconds == 1)
                    animeWatched += " **second**";
                else
                    animeWatched += " **seconds**";
            }

            return animeWatched;
        }
    }
}