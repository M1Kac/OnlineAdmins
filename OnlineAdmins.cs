using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CS2_GameHUDAPI;
using System.Numerics;
using CounterStrikeSharp.API.Core.Capabilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Drawing;

namespace OnlineAdmins
{
    public class OnlineAdmins : BasePlugin, IPluginConfig<Config>
    {
        public override string ModuleName => "Online Admins HUD";
        public override string ModuleAuthor => "M1K@c";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleDescription => "Displays a list of online admins on the screen.";

        private static IGameHUDAPI? _api;
        public required Config Config { get; set; }

        public void OnConfigParsed(Config config)
        {
            Config = config;
        }

        public override void OnAllPluginsLoaded(bool hotReload)
        {
            try
            {
                PluginCapability<IGameHUDAPI> CapabilityCP = new("gamehud:api");
                _api = IGameHUDAPI.Capability.Get();

                if (_api == null)
                {
                    PrintToConsole("GameHUDAPI not found. Make sure CS2-GameHUD is installed!");
                }
            }
            catch (Exception)
            {
                _api = null;
                PrintToConsole("Error loading GameHUDAPI.");
            }
        }

        [GameEventHandler]
        public HookResult OnPlayerSpawn(EventPlayerSpawn ev, GameEventInfo info)
        {
            var player = ev.Userid;
            if (player == null || !player.IsValid || _api == null) return HookResult.Continue;
            
            UpdateAdminHUD(player);
            return HookResult.Continue;
        }

    [ConsoleCommand("admins", "Show admins")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void ShowInfo(CCSPlayerController? controller, CommandInfo info)
    {
        if (_api == null || controller == null || !controller.IsValid || !controller.Pawn.IsValid || controller.IsBot || controller.IsHLTV) return;

         List<string> adminNames = Utilities.GetPlayers()
                .Where(p => p != null && p.IsValid && AdminManager.PlayerHasPermissions(p, Config.AdminFlag))
                .Select(p => p.PlayerName ?? "Unknown Admin")
                .ToList();

            string adminList = adminNames.Count > 0 
                ? "Online Admins:\n" + string.Join("\n", adminNames)
                : "No Admins Online";

            _api.Native_GameHUD_SetParams(
                controller, 
                6, 
                new CounterStrikeSharp.API.Modules.Utils.Vector(-30, 22, 40),
                Color.White, 
                80, 
                "Verdana", 
                0.018f);

            _api.Native_GameHUD_Show(controller, 6, adminList, Config.Time);

        }

        private void UpdateAdminHUD(CCSPlayerController player)
        {
            if (_api == null || player == null || !player.IsValid) return;
            
            List<string> adminNames = Utilities.GetPlayers()
                .Where(p => p != null && p.IsValid && AdminManager.PlayerHasPermissions(p, Config.AdminFlag))
                .Select(p => p.PlayerName ?? "Unknown Admin")
                .ToList();

            string adminList = adminNames.Count > 0 
                ? "Online Admins:\n" + string.Join("\n", adminNames)
                : "No Admins Online";

            _api.Native_GameHUD_SetParams(
                player, 
                6, 
                new CounterStrikeSharp.API.Modules.Utils.Vector(-30, 22, 40),
                Color.White, 
                80, 
                "Verdana", 
                0.018f);

            _api.Native_GameHUD_Show(player, 6, adminList, Config.Time);
        }

        public static void PrintToConsole(string sMessage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[OnlineAdmins] " + sMessage);
            Console.ResetColor();
        }
    }
}
