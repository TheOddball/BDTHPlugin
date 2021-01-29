﻿using Dalamud.Game.Command;
using Dalamud.Plugin;
using System;
using System.Threading;

namespace BDTHPlugin
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Burning Down the House";

        private const string commandName = "/bdth";

        private DalamudPluginInterface pi;
        private Configuration configuration;
        private PluginUI ui;
        private PluginMemory memory;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pi = pluginInterface;

            this.configuration = this.pi.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(this.pi);

            this.memory = new PluginMemory(this.pi);
            this.ui = new PluginUI(this.configuration, this.memory);

            this.pi.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the controls for Burning Down the House plugin."
            });

            this.pi.UiBuilder.OnBuildUi += DrawUI;
        }

        public void Dispose()
        {
            this.ui.Dispose();

            // Dispose for stuff in Plugin Memory class.
            this.memory.Dispose();

            this.pi.CommandManager.RemoveHandler(commandName);
            this.pi.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            args = args.Trim().ToLower();
            if(args != "")
            {
                var arg_list = args.Split(' ');
                var disabled = !(this.memory.IsHousingModeOn() && this.memory.selectedItem != IntPtr.Zero);
                if (arg_list.Length >= 3 && !disabled)
                {
                    float x = float.Parse(arg_list[0]);
                    float y = float.Parse(arg_list[1]);
                    float z = float.Parse(arg_list[2]);
                    this.memory.position.X = x;
                    this.memory.position.Y = y;
                    this.memory.position.Z = z;
                    this.memory.WritePosition(this.memory.position);
                    if(arg_list.Length == 4)
                    {
                        this.memory.rotation = double.Parse(arg_list[3]);
                        this.memory.WriteRotation(this.memory.rotation);
                    }
                }
            }
            else
            {
                // in response to the slash command, just display our main ui
                this.ui.Visible = true;
            }
        }

        private void DrawUI()
        {
            this.ui.Draw();
        }
    }
}
