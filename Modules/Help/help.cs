using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperBot_2._0.Modules.Help
{
    public class Help : ModuleBase
    {
        private readonly CommandService _service;

        public Help(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("**help**?")]
        public async Task HelpAsync([Remainder]string command = null)
        {
            var builder = new EmbedBuilder();
            try
            {
                if (command == null)
                {
                    builder.Title = "These are the modules you can use";
                    builder.Description = "To see the commands in a module use %module (module name)";
                    builder.Color = new Color(114, 137, 218);
                    var modules = new List<string>();
                    int modulecount = 0;
                    int commandcount = 0;
                    foreach (ModuleInfo module in _service.Modules)
                    {
                        if (!modules.Contains(module.Name) || !module.IsSubmodule)
                        {
                            modules.Add(module.Name);
                            modulecount++;
                            commandcount += module.Commands.Count;
                        }
                    }
                    builder.AddField(x =>
                    {
                        x.Name = modulecount + " Modules total commands " + commandcount;
                        x.Value = string.Join("\n", modules);
                        x.IsInline = true;
                    });
                    await ReplyAsync("", false, builder.Build());
                }
                else
                {
                    var result = _service.Search(Context, command.Replace("%", ""));

                    if (!result.IsSuccess)
                    {
                        await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                        return;
                    }
                    foreach (CommandMatch match in result.Commands)
                    {
                        var cmd = match.Command;

                        builder.AddField(x =>
                        {
                            x.Name = string.Join(", ", cmd.Aliases);
                            x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Type.Name + " " + p.Name))}\n" +
                                      $"Summary: {cmd.Summary}";
                            x.IsInline = false;
                        });
                    }

                    await ReplyAsync("", false, builder.Build());
                }
            }
            catch (Exception ex)
            {
                builder.Color = Color.Red;
                builder.AddField(x =>
                {
                    x.Name = "error";
                    x.Value = ex.Message;
                    x.IsInline = true;
                });
                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("module")]
        [Summary("Gets the commands of a module")]
        public async Task Helpasync(string module)
        {
            string prefix = "%";
            var builder = new EmbedBuilder();
            var commands = new List<string>();
            foreach (var match in _service.Modules)
            {
                if (match.Name.ToLower() == module.ToLower())
                {
                    foreach (var cmd in match.Commands)
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess)
                            if (!commands.Contains(prefix + cmd.Aliases.First()))
                                commands.Add($"{prefix}{cmd.Aliases.First()}");
                    }
                }
            }
            if (commands.Count > 0)
            {
                builder.Color = new Color(114, 137, 218);
                builder.Description = $"These are the commands in **{module}**";
                builder.AddField(x =>
                {
                    x.Name = module;
                    x.Value = string.Join("\n", commands);
                    x.IsInline = false;
                });
            }
            else
            {
                builder.Color = Color.Red;
                builder.AddField("Error", "I couldn't find the module");
            }
            await ReplyAsync("", false, builder.Build());
        }
    }
}