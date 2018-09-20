using Discord.Commands;
using Newtonsoft.Json;
using RestSharp;
using StrawPollNET.Enums;
using SuperBot_2._0.Services;
using SuperBotDLL1_0.Classes.GuildUntils;
using SuperBotDLL1_0.RankingSystem;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SuperBot_2._0.Modules.Usefull
{
    public class Commands : ModuleBase
    {
        [Command("test"), RequireOwner]
        public async Task Test()
        {
            //XmlDocument doc = new XmlDocument();
            //doc.Load($"./file/ranks/users/{Context.User.Id}.xml");
            TestJson guild = new TestJson();
            //GuildChannel guild = new GuildChannel(Context.Guild);
            string output = JsonConvert.SerializeObject(guild);

            using (StreamWriter writer = new StreamWriter("./file/jsontext.json"))
            {
                writer.Write(output);
            }
            await ReplyAsync(DateTime.Now.ToLongTimeString());
        }

        [Command("jessie")]
        public async Task Jessie()
        {
            await ReplyAsync("Jessie it the best!");
        }

        [Command("strawpoll")]
        public async Task Strawpoll(string title, params string[] options)
        {
            try
            {
                var poll = new Strawpoll();

                var obj = new PollRequest()
                {
                    Title = title,
                    Options = new List<string>() { options[0], options[1] },
                    Multi = false,
                    Dupcheck = DupCheck.Normal,
                    Captcha = false
                };

                var p = await poll.CreatePollAsync(obj);
                //var pp = await poll.GetPollAsync(p.Id);

                await ReplyAsync(p.Id.ToString());
            }
            catch (Exception ex)
            {
                await ReplyAsync("error: " + ex.Message.ToString());
            }
        }

        public IRestResponse response(string url)
        {
            var client = new RestClient()
            {
                BaseUrl = new Uri(url)
            };
            var request = new RestRequest()
            {
                Method = Method.GET
            };
            return client.Execute(request);
        }

        public Image RoundCorners(Image img)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddEllipse(0, 0, img.Width, img.Height);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.SetClip(gp);
                    gr.DrawImage(img, Point.Empty);
                }
            }
            return bmp;
        }

        private class TextDrawing
        {
            public enum DrawMethod
            {
                AutosizeAccordingToText, // create the smallest bitmap needed to draw the text without word warp
                AutoFitInConstantRectangleWithoutWarp, // draw text with the biggest font possible while not exceeding rectangle dimensions, without word warp
                AutoWarpInConstantRectangle, // draw text in rectangle while performing word warp. font size is a constant input. drawing may exceed bitmap rectangle.
                AutoFitInConstantRectangleWithWarp // draw text with the biggest font possible while not exceeding rectangle dimensions, with word warp
            }

            private static void SetGraphicsHighQualityForTextRendering(Graphics g)
            {
                // The smoothing mode specifies whether lines, curves, and the edges of filled areas use smoothing (also called antialiasing). One exception is that path gradient brushes do not obey the smoothing mode. Areas filled using a PathGradientBrush are rendered the same way (aliased) regardless of the SmoothingMode property.
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // The interpolation mode determines how intermediate values between two endpoints are calculated.
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Use this property to specify either higher quality, slower rendering, or lower quality, faster rendering of the contents of this Graphics object.
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // This one is important
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            }

            public static Size MeasureDrawTextBitmapSize(string text, Font font)
            {
                Bitmap bmp = new Bitmap(1, 1);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    SizeF size = g.MeasureString(text, font);
                    return new Size((int)(Math.Ceiling(size.Width)), (int)(Math.Ceiling(size.Height)));
                }
            }

            public static int GetMaximumFontSizeFitInRectangle(string text, Font font, RectangleF rectanglef, bool isWarp, int MinumumFontSize = 6, int MaximumFontSize = 1000)
            {
                Font newFont;
                Rectangle rect = Rectangle.Ceiling(rectanglef);

                for (int newFontSize = MinumumFontSize; ; newFontSize++)
                {
                    newFont = new Font(font.FontFamily, newFontSize, font.Style);

                    List<string> ls = WarpText(text, newFont, rect.Width);

                    StringBuilder sb = new StringBuilder();
                    if (isWarp)
                    {
                        for (int i = 0; i < ls.Count; ++i)
                        {
                            sb.Append(ls[i] + Environment.NewLine);
                        }
                    }
                    else
                    {
                        sb.Append(text);
                    }

                    Size size = MeasureDrawTextBitmapSize(sb.ToString(), newFont);
                    if (size.Width > rectanglef.Width || size.Height > rectanglef.Height)
                    {
                        return (newFontSize - 1);
                    }
                    if (newFontSize >= MaximumFontSize)
                    {
                        return (newFontSize - 1);
                    }
                }
            }

            public static List<string> WarpText(string text, Font font, int lineWidthInPixels)
            {
                string[] originalLines = text.Split(new string[] { " " }, StringSplitOptions.None);

                List<string> wrappedLines = new List<string>();

                StringBuilder actualLine = new StringBuilder();
                double actualWidthInPixels = 0;

                foreach (string str in originalLines)
                {
                    Size size = MeasureDrawTextBitmapSize(str, font);

                    actualLine.Append(str + " ");
                    actualWidthInPixels += size.Width;

                    if (actualWidthInPixels > lineWidthInPixels)
                    {
                        actualLine = actualLine.Remove(actualLine.ToString().Length - str.Length - 1, str.Length);
                        wrappedLines.Add(actualLine.ToString());
                        actualLine.Clear();
                        actualLine.Append(str + " ");
                        actualWidthInPixels = size.Width;
                    }
                }

                if (actualLine.Length > 0)
                {
                    wrappedLines.Add(actualLine.ToString());
                }

                return wrappedLines;
            }

            public static Bitmap DrawTextToBitmap(string text, Font font, Color color, DrawMethod mode, RectangleF rectanglef)
            {
                StringFormat drawFormat = new StringFormat();
                Bitmap bmp;
                switch (mode)
                {
                    case DrawMethod.AutosizeAccordingToText:
                        {
                            Size size = MeasureDrawTextBitmapSize(text, font);

                            if (size.Width == 0 || size.Height == 0)
                            {
                                bmp = new Bitmap(1, 1);
                            }
                            else
                            {
                                bmp = new Bitmap(size.Width, size.Height);
                            }

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                SetGraphicsHighQualityForTextRendering(g);

                                g.DrawString(text, font, new SolidBrush(color), 0, 0);

                                return bmp;
                            }
                        }
                    case DrawMethod.AutoWarpInConstantRectangle:
                        {
                            Rectangle rect = Rectangle.Ceiling(rectanglef);
                            bmp = new Bitmap(rect.Width, rect.Height);

                            if (rect.Width == 0 || rect.Height == 0)
                            {
                                bmp = new Bitmap(1, 1);
                            }
                            else
                            {
                                bmp = new Bitmap(rect.Width, rect.Height);
                            }

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                SetGraphicsHighQualityForTextRendering(g);

                                g.DrawString(text, font, new SolidBrush(color), rectanglef, drawFormat);

                                return bmp;
                            }
                        }
                    case DrawMethod.AutoFitInConstantRectangleWithoutWarp:
                        {
                            Rectangle rect = Rectangle.Ceiling(rectanglef);

                            bmp = new Bitmap(rect.Width, rect.Height);

                            if (rect.Width == 0 || rect.Height == 0)
                            {
                                bmp = new Bitmap(1, 1);
                            }
                            else
                            {
                                bmp = new Bitmap(rect.Width, rect.Height);
                            }

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                int fontSize = GetMaximumFontSizeFitInRectangle(text, font, rectanglef, false);

                                SetGraphicsHighQualityForTextRendering(g);

                                g.DrawString(text, new Font(font.FontFamily, fontSize, font.Style, GraphicsUnit.Point), new SolidBrush(color), rectanglef, drawFormat);

                                return bmp;
                            }
                        }
                    case DrawMethod.AutoFitInConstantRectangleWithWarp:
                        {
                            Rectangle rect = Rectangle.Ceiling(rectanglef);

                            if (rect.Width == 0 || rect.Height == 0)
                            {
                                bmp = new Bitmap(1, 1);
                            }
                            else
                            {
                                bmp = new Bitmap(rect.Width, rect.Height);
                            }

                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                int fontSize = GetMaximumFontSizeFitInRectangle(text, font, rectanglef, true);

                                SetGraphicsHighQualityForTextRendering(g);

                                g.DrawString(text, new Font(font.FontFamily, fontSize, font.Style, GraphicsUnit.Point), new SolidBrush(color), rectanglef, drawFormat);

                                return bmp;
                            }
                        }
                }
                return null;
            }
        }

        //string[] Scopes = { DriveService.Scope.DriveReadonly };
        //string ApplicationName = "Drive API .NET Quickstart";

        //UserCredential credential;

        //using (var stream =
        //    new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
        //{
        //    string credPath = System.Environment.GetFolderPath(
        //        System.Environment.SpecialFolder.Personal);
        //    credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

        //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //        GoogleClientSecrets.Load(stream).Secrets,
        //        Scopes,
        //        "user",
        //        CancellationToken.None,
        //        new FileDataStore(credPath, true)).Result;
        //    Console.WriteLine("Credential file saved to: " + credPath);
        //}

        //// Create Drive API service.
        //var service = new DriveService(new BaseClientService.Initializer()
        //{
        //    HttpClientInitializer = credential,
        //    ApplicationName = ApplicationName,
        //});

        //// Define parameters of request.
        //FilesResource.ListRequest listRequest = service.Files.List();
        //listRequest.PageSize = 10;
        //listRequest.Fields = "nextPageToken, files(id, name)";

        //// List files.
        //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
        //    .Files;
        //Console.WriteLine("Files:");
        //if (files != null && files.Count > 0)
        //{
        //    foreach (var file in files)
        //    {
        //        Console.WriteLine("{0} ({1})", file.Name, file.Id);
        //    }
        //}
        //else
        //{
        //    Console.WriteLine("No files found.");
        //}

        //string key = "trnsl.1.1.20180304T105743Z.5a708ae4441191e9.692311a4087294698c179e05d0588c507654861d";
        //string UrlDetectSrsLanguage = $@"https://translate.yandex.net/api/v1.5/tr.json/detect?key={key}&text={text}";
        //string Urltranslate = $@"https://translate.yandex.net/api/v1.5/tr.json/translate?key={key}&text={text}&lang=en";
        //var Response = response(Urltranslate);
        //var json = JsonConvert.DeserializeObject<IDictionary>(Response.Content);
        //IUser user = Program._client.GetUser(Context.User.Id);
        //await ReplyAsync(json["text"].ToString().Replace("[", "").Replace("]", "").Replace("\"", ""));
    }
}