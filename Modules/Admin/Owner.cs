using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace SuperBot_2._0.Modules.Admin
{
    [Name("Owner")]
    public class Owner
    {
        [Command("restart")]
        public async Task Restartbot()
        {
            await Task.Delay(1);
        }
    }
}
