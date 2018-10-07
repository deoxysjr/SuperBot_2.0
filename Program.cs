using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using SuperBot_2._0.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SuperBot_2_0
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "SuperBot 2.0";
            Console.SetBufferSize(Console.BufferWidth + 10, Console.BufferHeight - 150);
            Console.SetWindowSize(Console.WindowWidth + 10, Console.WindowHeight);
            new Program().MainAsync().GetAwaiter().GetResult();
            if (_client.LoginState == LoginState.LoggedOut)
                new Program().MainAsync().GetAwaiter().GetResult();
        }

        public static DiscordSocketClient _client;
        public static readonly CommandService _commands = new CommandService();
        public static DateTime StartupTime = DateTime.Now;
        public static IServiceProvider _services;
        public readonly static string levelpath = "./file/ranks/users/";
        public static string[] mineinv = { "stone", "goldore", "ironore", "gem", "coal", "oil", "sand" };
        public static string[] baginv = { "apple", "avocado", "banana", "carrot", "cherries", "chillies", "corn",
            "cucumber", "egg", "eggplant", "grain", "grape", "kiwi", "lemon", "melon", "milk", "orange",
            "peach", "peanuts", "pear", "pineapple", "potato", "strawberry", "sugarcane", "tomato" };
        public static string[] craftlist = { "gold", "iron", "ring", "crown", "bakedegg", "flour", "sugar", "glass", "refinedoil" };
        public static Dictionary<string, int> mineprices = new Dictionary<string, int>() { { "gem", 100 }, { "stone", 15 }, { "goldore", 25 }, { "ironore", 20 }, { "coal", 25 },
            { "oil", 150 }, { "sand", 15 } };
        public static Dictionary<string, int> pickprices = new Dictionary<string, int>() { {"apple", 20}, {"avocado", 15}, {"banana", 20}, {"carrot", 25}, {"cherries", 20},
            { "chillies", 15}, {"corn", 20}, {"cucumber", 25}, { "egg", 10 }, {"eggplant", 25}, { "grain", 15 }, {"grape", 20}, {"kiwi", 20}, {"lemon", 25}, {"melon", 25}, { "milk", 20 }, {"orange", 20}, {"peach", 20},
                {"peanuts", 15}, {"pear", 20}, {"pineapple", 25}, {"potato", 25}, {"strawberry", 20}, { "sugarcane", 25 }, {"tomato", 20} };
        public static Dictionary<string, int> craftprice = new Dictionary<string, int>() { { "gold", 300 }, { "iron", 250 }, { "crown", 500 }, { "ring", 450 }, { "flower", 75 }, { "flour", 75 },
                                           { "sugar", 50 }, { "glass", 100 }, { "refinedoil", 200 } };

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                WebSocketProvider = WS4NetProvider.Instance
            });
            _client.Log += Logger;
            _commands.Log += Logger;
        }

        private static Task Logger(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message}" /*{message.Exception}"*/);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;

            File.AppendAllText("error.txt", $"{DateTime.Now,-19} [{message.Severity}] {message.Source}: {message.Message}\r\n");

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await Handlers.InitHandlers();
            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("./discordkey.txt"));
            await _client.StartAsync();

            await Task.Delay(4000);
            await _client.SetGameAsync(GetAllUsers(_client), null, StreamType.Twitch);
            await Task.Delay(-1);
        }

        private string GetAllUsers(DiscordSocketClient client)
        {
            string output = "Guild users ";
            int usercount = 0;
            foreach (var guild in _client.Guilds)
                usercount += guild.Users.Count;
            output += usercount;
            return output;
        }
    }
}