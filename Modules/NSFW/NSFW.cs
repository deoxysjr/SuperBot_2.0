using Discord;
using Discord.Commands;
using SuperBot_2_0.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace SuperBot_2_0.Modules.NSFW
{
    public class NSFW : ModuleBase
    {
        [Command("rule34")]
        public async Task Rule34([Remainder]string tags = null)
        {
            var builder = new EmbedBuilder();
            if (Context.Channel.Name.ToLower().Contains("nsfw"))
            {
                if (tags == null)
                {
                    string tag;
                    int amount = 0;
                    var list = new List<string>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"./tags.xml");
                    foreach (XmlNode node in doc.SelectNodes("root/rule34/tag"))
                    {
                        list.Add(node.InnerText);
                        amount++;
                    }
                    tag = list[Utils.getRandInt(1, amount - 1)];
                    var url = await Utils.SearchRule34(tag).ConfigureAwait(false);

                    if (url == null)
                    {
                        builder.Color = Color.Red;
                        builder.Title = "**No results**";
                        await ReplyAsync("", false, builder.Build());
                        Console.Write($"[{Utils.CurrentTime()}] ");
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"No results");
                        Console.ResetColor();
                    }
                    else
                    {
                        if (url.Contains(".webm"))
                        {
                            string[] urltags = url.Split('`');
                            builder.Color = Color.Green;
                            builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                            builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                            builder.WithDescription(urltags[0]);
                            await ReplyAsync("", false, builder.Build());
                        }
                        else
                        {
                            string[] urltags = url.Split('`');
                            builder.Color = Color.Green;
                            builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                            builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                            builder.WithImageUrl(urltags[0]);
                            await ReplyAsync("", false, builder.Build());
                        }
                    }
                }
                else
                {
                    string args = tags;
                    if (args.ToLower().Contains("tags"))
                    {
                        await ReplyAsync("https://rule34.xxx/index.php?page=tags&s=list");
                    }
                    else
                    {
                        string tag;
                        if (args == "")
                        {
                            int amount = 0;
                            var list = new List<string>();
                            XmlDocument doc = new XmlDocument();
                            doc.Load(@"./tags.xml");
                            foreach (XmlNode node in doc.SelectNodes("root/rule34/tag"))
                            {
                                list.Add(node.InnerText);
                                amount++;
                            }
                            tag = list[Utils.getRandInt(1, amount - 1)];
                        }
                        else
                        {
                            tag = string.Join("_", args);
                        }

                        var url = await Utils.SearchRule34(tag).ConfigureAwait(false);

                        if (url == null)
                        {
                            builder.Color = Color.Red;
                            builder.Title = "**No results**";
                            await ReplyAsync("", false, builder.Build());
                            Console.Write($"[{Utils.CurrentTime()}] ");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"No results");
                            Console.ResetColor();
                        }
                        else
                        {
                            if (url.Contains(".webm"))
                            {
                                string[] urltags = url.Split('`');
                                builder.Color = Color.Green;
                                builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                                builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                                builder.WithDescription(urltags[0]);
                                await ReplyAsync("", false, builder.Build());
                            }
                            else
                            {
                                string[] urltags = url.Split('`');
                                builder.Color = Color.Green;
                                builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                                builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                                builder.WithImageUrl(urltags[0]);
                                await ReplyAsync("", false, builder.Build());
                            }
                        }
                    }
                }
            }
            else
            {
                builder.Color = Color.Red;
                builder.AddField("Sorry", "could you please do this in a NSFW channel");
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("yandere")]
        public async Task Yandere([Remainder]string tags = null)
        {
            var builder = new EmbedBuilder();
            if (Context.Channel.Name.ToLower().Contains("nsfw"))
            {
                if (tags == null)
                {
                    string tag = "";
                    int amount = 0;
                    var list = new List<string>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(@"./tags.xml");
                    foreach (XmlNode node in doc.SelectNodes("root/yandere/tag"))
                    {
                        list.Add(node.InnerText);
                        amount++;
                    }
                    tag = list[Utils.getRandInt(0, amount - 1)];
                    var url = await Utils.SearchYandere(tag).ConfigureAwait(false);

                    if (url == null)
                    {
                        builder.Color = Color.Red;
                        builder.Title = "**No results**";
                        await ReplyAsync("", false, builder.Build());
                        Console.Write($"[{Utils.CurrentTime()}] ");
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"No results");
                        Console.ResetColor();
                    }
                    else
                    {
                        string[] urltags = url.Split('`');
                        builder.Color = Color.Green;
                        builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                        builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                        builder.WithImageUrl(urltags[0]);
                        await ReplyAsync("", false, builder.Build());
                    }
                }
                else
                {
                    string args = tags;
                    string tag = "";
                    if (args == "")
                    {
                        int amount = 0;
                        var list = new List<string>();
                        XmlDocument doc = new XmlDocument();
                        doc.Load(@"./tags.xml");
                        foreach (XmlNode node in doc.SelectNodes("root/yandere/tag"))
                        {
                            list.Add(node.InnerText);
                            amount++;
                        }
                        tag = list[Utils.getRandInt(0, amount - 1)];
                    }
                    else
                    {
                        tag = string.Join("_", args);
                    }
                    var url = await Utils.SearchYandere(tag).ConfigureAwait(false);

                    if (url == null)
                    {
                        builder.Color = Color.Red;
                        builder.Title = "**No results**";
                        await ReplyAsync("", false, builder.Build());
                        Console.Write($"[{Utils.CurrentTime()}] ");
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"No results");
                        Console.ResetColor();
                    }
                    else
                    {
                        string[] urltags = url.Split('`');
                        builder.Color = Color.Green;
                        builder.Title = $"I found this Image with the tag or tags: **{tag}**";
                        builder.Description = $"{urltags[1].Replace(" ", ", ")}";
                        builder.WithImageUrl(urltags[0]);
                        await ReplyAsync("", false, builder.Build());
                    }
                }
            }
            else
            {
                builder.Color = Color.Red;
                builder.AddField("Sorry", "could you please do this in a NSFW channel");
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("hentai")]
        public async Task Hentai()
        {
            try
            {
                Random rand = new Random();
                string[] files = Directory.GetFiles(@"E:/Afbeeldingen/uitzoeken");
                string path = files[rand.Next(files.Length)];
                bool tobig = true;
                string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
                while (tobig)
                {
                    long bytes = new FileInfo(path).Length;
                    string length = Utils.SizeSuffix(bytes, SizeSuffixes);
                    Console.WriteLine(length);
                    if (length.Contains("KB") || length.Contains("bytes"))
                        tobig = false;
                    else if (length.Contains("MB"))
                    {
                        double size = double.Parse(length.Split(' ')[0]);
                        if (size < 8)
                            tobig = false;
                        else
                            path = files[rand.Next(files.Length)];
                    }
                    else
                        path = files[rand.Next(files.Length)];
                }
                await Context.Channel.SendFileAsync(path);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.ToString());
            }
        }
    }
}