using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using SuperBot_2_0.Services;
using SuperBotDLL1_0;
using SuperBotDLL1_0.Classes.Encryption;
using SuperBotDLL1_0.color;
using SuperBotDLL1_0.Untils.ForecastClasses;
using SuperBotDLL1_0.Untils.WeatherClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SuperBot_2_0.Modules.Usefull
{
    public class Usefull : ModuleBase<ICommandContext>
    {
        [Command("weather")]
        public async Task Weather([Remainder]string city)
        {
            if (city.Contains("_"))
                city.Replace('_', ' ');
            try
            {
                WeatherAPIcall weather = new WeatherAPIcall(city);
                if (weather.ValidRequest)
                    await ReplyAsync("", false, weather.GetEmbed());
            }
            catch (Exception ex)
            {
                var builder = new EmbedBuilder();
                if (ex.Message.Contains("404"))
                {
                    builder.AddField("Error", $"error id: 404\nerror message: City not found");
                }
                else
                {
                    builder.AddField(x =>
                    {
                        x.Name = "Error";
                        x.Value = ex.ToString();
                        x.IsInline = false;
                    });
                }
                builder.Color = Discord.Color.Red;

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("forecast")]
        public async Task Forecast([Remainder]string city)
        {
            try
            {
                ForecastAPIcall forecast = new ForecastAPIcall(city);
                await ReplyAsync("", false, forecast.GetEmbed());
            }
            catch (Exception ex)
            {
                var builder = new EmbedBuilder();
                builder.Color = Discord.Color.Red;
                builder.AddField(x =>
                {
                    x.Name = "Error";
                    x.Value = ex.ToString();
                    x.IsInline = false;
                });
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("color")]
        public async Task ColorSend(params string[] arg)
        {
            string img = @".\1024x1024.jpg";
            Bitmap bmp = new Bitmap(img);
            string arg1 = arg[0];
            int width = bmp.Width;
            int height = bmp.Height;

            if (arg[0].Contains("#"))
            {
                string colorcode = arg[0];
                var code = colorcode.Replace("#", "#ff");
                System.Drawing.Color color = ColorTranslator.FromHtml($"{code}");

                int R = color.R;
                int G = color.G;
                int B = color.B;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        System.Drawing.Color p = bmp.GetPixel(x, y);

                        int a = p.A;

                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, R, G, B));
                    }
                }
                try
                {
                    bmp.Save(@".\\color.png");
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex);
                }
                await Context.Channel.SendFileAsync(@".\color.png");
                if (File.Exists(@"./color.png"))
                {
                    File.Delete(@"./color.png");
                }
                return;
            }
            else if (arg[1] != "")
            {
                int R = int.Parse(arg[0]);
                int G = int.Parse(arg[1]);
                int B = int.Parse(arg[2]);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        System.Drawing.Color p = bmp.GetPixel(x, y);

                        int a = p.A;

                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(a, R, G, B));
                    }
                }
                try
                {
                    bmp.Save(@".\\color.png");
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex);
                }
                await Context.Channel.SendFileAsync(@".\color.png");
                if (File.Exists(@"./color.png"))
                {
                    File.Delete(@"./color.png");
                }
            }
            else
            {
                switch (arg1)
                {
                    case "red":
                        Sendcolor.ColorRed(width, height, Context, bmp);
                        break;

                    case "lightred":
                        Sendcolor.ColorLightRed(width, height, Context, bmp);
                        break;

                    case "green":
                        Sendcolor.ColorGreen(width, height, Context, bmp);
                        break;

                    case "lightgreen":
                        Sendcolor.ColorLightGreen(width, height, Context, bmp);
                        break;

                    case "blue":
                        Sendcolor.ColorBlue(width, height, Context, bmp);
                        break;

                    case "lightblue":
                        Sendcolor.ColorLightBlue(width, height, Context, bmp);
                        break;

                    case "black":
                    case "nigger":
                        Sendcolor.ColorBlack(width, height, Context, bmp);
                        break;

                    case "white":
                        Sendcolor.ColorWhite(width, height, Context, bmp);
                        break;

                    case "lightgray":
                        Sendcolor.ColorLightGray(width, height, Context, bmp);
                        break;

                    case "gray":
                        Sendcolor.ColorGray(width, height, Context, bmp);
                        break;

                    case "yellow":
                        Sendcolor.ColorYellow(width, height, Context, bmp);
                        break;

                    case "orange":
                        Sendcolor.ColorOrange(width, height, Context, bmp);
                        break;

                    case "purple":
                        Sendcolor.ColorPurple(width, height, Context, bmp);
                        break;

                    case "random":
                        Sendcolor.ColorRandom(width, height, Context, bmp);
                        break;

                    case "randomall":
                        Sendcolor.ColorRandomAll(width, height, Context, bmp);
                        break;

                    case "randomvert":
                        Sendcolor.ColorRandomVret(width, height, Context, bmp);
                        break;

                    case "randomhor":
                        Sendcolor.ColorRandomHor(width, height, Context, bmp);
                        break;

                    default:
                        break;
                }
            }
        }

        [Command("convert")]
        public async Task Convert(params string[] code)
        {
            if (code[0].Contains("#"))
            {
                System.Drawing.Color color = ColorTranslator.FromHtml($"{code[0]}");
                int R = color.R;
                int G = color.G;
                int B = color.B;

                await ReplyAsync($"R:{R} G:{G} B:{B}");
            }
            else
            {
                int r = int.Parse(code[0]);
                int g = int.Parse(code[1]);
                int b = int.Parse(code[2]);

                System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);

                string hex = color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

                await ReplyAsync($"%color #{hex}");
            }
        }

        #region useless commands

        //[Command("plot")]
        //public async Task Plot()
        //{
        //    Bitmap myBitmap = new Bitmap(@"./1024x1024.jpg");
        //    Graphics g = Graphics.FromImage(myBitmap);

        //    g.DrawLine(Pens.Gray, 1, 1, 200, 400);
        //    myBitmap.Save(@"./text.png");
        //    await Context.Channel.SendFileAsync(@"./text.png");
        //    if (File.Exists(@"./text.png"))
        //    {
        //        File.Delete(@"./text.png");
        //    }
        //}

        //[Command("draw")]
        //public async Task Draw(string arg)
        //{
        //    Bitmap myBitmap = new Bitmap(@"./1024x1024.jpg");
        //    Graphics g = Graphics.FromImage(myBitmap);

        //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        //    g.DrawString($"{arg}", new Font("Alien Encounters", 30), Brushes.Black, new PointF(50, 50));
        //    myBitmap.Save(@"./text.png");
        //    await Context.Channel.SendFileAsync(@"./text.png");
        //    if (File.Exists(@"./text.png"))
        //    {
        //        File.Delete(@"./text.png");
        //    }
        //}

        #endregion useless commands

        [Command("anime")]
        public async Task SearchAnime([Remainder]string anime)
        {
            string url = $@"https://aniapi.nadekobot.me/anime/{anime}";
            string res;
            var list = new List<string>();
            var builder = new EmbedBuilder()
            {
                Color = new Discord.Color(0, 255, 0)
            };

            try
            {
                using (var http = new HttpClient())
                {
                    res = await http.GetStringAsync(url).ConfigureAwait(false);
                    var json = JsonConvert.DeserializeObject<dynamic>(res);
                    string eng = json.title_english.ToString();
                    string jap = json.title_japanese.ToString();
                    string rom = json.title_romaji.ToString();
                    if (eng == jap && eng == rom && jap == rom)
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                    else if (eng == jap && eng != rom && jap == rom)
                    {
                        list.Add("English Title: " + json.title_english.ToString());
                        list.Add("Romaji title: " + json.title_romaji.ToString());
                    }
                    else if (eng == jap && eng != rom && jap != rom)
                    {
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                        list.Add("Romaji title: " + json.title_romaji.ToString());
                    }
                    else if (eng == jap && eng == rom && jap != rom)
                    {
                        list.Add("English Title: " + json.title_english.ToString());
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                    }
                    else if (eng != jap && eng == rom && jap == rom)
                    {
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                        list.Add("Romaji title: " + json.title_romaji.ToString());
                    }
                    else if (eng != jap && eng == rom && jap != rom)
                    {
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                        list.Add("Romaji title: " + json.title_romaji.ToString());
                    }
                    else if (eng != jap && eng != rom && jap == rom)
                    {
                        list.Add("English Title: " + json.title_english.ToString());
                        list.Add("Japanese title: " + json.title_japanese.ToString());
                    }
                    //list.Add("English Title: " + json.title_english.ToString());
                    //list.Add("Japanese title: " + json.title_japanese.ToString());
                    //list.Add("Romaji title: " + json.title_romaji.ToString());
                    list.Add("Synonyms: " + string.Join(", ", json.synonyms));
                    list.Add("Type: " + json.type.ToString());
                    list.Add("Average score: " + json.average_score.ToString());
                    list.Add("status: " + json.airing_status.ToString());
                    list.Add("Episodes: " + json.total_episodes.ToString());
                    list.Add("Duration: " + json.duration.ToString());
                    list.Add("Total time: " + TotalTime(json.total_episodes.ToString(), json.duration.ToString()));
                    list.Add("Adult: " + json.adult);
                    string description = json.description.ToString().Replace("<br>", "");
                    builder.AddField(x =>
                    {
                        x.Name = $"**{anime}**";
                        x.Value = string.Join("\n", list);
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "**Description**";
                        x.Value = description;
                        x.IsInline = true;
                    });
                    //builder.WithImageUrl(json.image_url_lge.ToString());
                    builder.ImageUrl = json.image_url_lge.ToString();
                }
            }
            catch (Exception ex)
            {
                builder.Fields.Clear();
                builder.Color = new Discord.Color(255, 0, 0);
                builder.AddField(x =>
                {
                    x.Name = "**Error**";
                    x.Value = ex.ToString();
                    x.IsInline = false;
                });
                await ReplyAsync("", false, builder.Build());
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("statistics")]
        public async Task BotData()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Bot Statistics";

            SQLConnection Connection = new SQLConnection();
            Connection.ExcuteCommand("");

            builder.AddField("total info", "");

            await ReplyAsync("", false);
            Connection.CloseConnection();
        }

        [Command("Encrypt"), RequireOwner]
        public async Task Encrypt([Remainder]string text)
        {
            File.AppendAllText("./encryptedmessage.txt", Encryption.EncryptText(text));
            await Context.Channel.SendFileAsync("./encryptedmessage.txt");
            if (File.Exists("./encryptedmessage.txt"))
                File.Delete("./encryptedmessage.txt");
        }

        [Command("Decrypt"), RequireOwner]
        public async Task Decrypt([Remainder]string text)
        {
            try
            {
                string output = Encryption.ToText(text);
                if (output.Contains("error"))
                    await ReplyAsync(output);
                else
                {
                    File.AppendAllText("./decryptedmessage.txt", output);
                    await Context.Channel.SendFileAsync("./decryptedmessage.txt");
                    if (File.Exists("./decryptedmessage.txt"))
                        File.Delete("./decryptedmessage.txt");
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        public string TotalTime(string eps, string dur)
        {
            int time_in_sec = int.Parse(dur) * 60;
            int Totaltime = time_in_sec * int.Parse(eps);
            return Utils.CalculateTimeWithSeconds(Totaltime);
        }
    }
}