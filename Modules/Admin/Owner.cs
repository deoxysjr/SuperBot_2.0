using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace SuperBot_2._0.Modules.Admin
{
    [Name("Owner")]
    public class Owner : ModuleBase
    {
        [Command("restart")]
        public async Task Restartbot()
        {
            await ReplyAsync("Restarting SuperBot");
            await Task.Delay(1000);
            Process.Start(@"D:\Super Bot\SuperBot_2.0\SuperBot_2.0\ReStartSuperbot\bin\Debug\ReStartSuperbot.exe");
        }
    }
}
