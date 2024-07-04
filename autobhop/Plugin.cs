using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Dalamud.Game.ClientState.Conditions;
using System.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace autobhop
{
    public sealed class Plugin : IDalamudPlugin
    {
        private IDalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        private bool InFlight => Service.Condition[ConditionFlag.InFlight];

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        private readonly IntPtr ffxivWindowHandle;

        public Plugin(
            IDalamudPluginInterface pluginInterface,
            ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.PluginInterface.Create<Service>(this);
            ECommonsMain.Init(pluginInterface, this, Module.All);

            this.ffxivWindowHandle = Process.GetCurrentProcess().MainWindowHandle;

            Service.Framework.Update += onFrameworkUpdate;
        }

        public void onFrameworkUpdate(IFramework framework)
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow == this.ffxivWindowHandle)
            {
                if (GenericHelpers.IsKeyPressed(Keys.Space) && !InFlight)
                {
                    ECommons.Automation.WindowsKeypress.SendKeypress(Keys.Space);
                }
            }
        }

        public void Dispose()
        {
            Service.Framework.Update -= onFrameworkUpdate;
        }
    }
}
