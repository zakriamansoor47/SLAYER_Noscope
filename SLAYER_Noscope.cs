using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using System.Text.Json.Serialization;
using System.Drawing;
using Microsoft.Extensions.Logging;
using Serilog;


namespace SLAYER_Noscope;

// Used these to remove compile warnings

public class SLAYER_NoscopeConfig : BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
    [JsonPropertyName("AlwaysDisableScope")] public bool AlwaysDisableScope { get; set; } = false;
    [JsonPropertyName("PlayerCanUseNsCmd")] public bool PlayerCanUseNsCmd { get; set; } = true;
    [JsonPropertyName("ShowYouCantScopeMsg")] public bool ShowYouCantScopeMsg { get; set; } = true;
    [JsonPropertyName("BulletTracers")] public bool BulletTracers { get; set; } = true;
    [JsonPropertyName("AdminFlagtoForceNS")] public string AdminFlagtoForceNS { get; set; } = "@css/root";
}

public class SLAYER_Noscope : BasePlugin, IPluginConfig<SLAYER_NoscopeConfig>
{
    public override string ModuleName => "SLAYER_Noscope";
    public override string ModuleVersion => "1.3";
    public override string ModuleAuthor => "SLAYER";
    public override string ModuleDescription => "Enable/Disable Noscope for all Scope Weapons";
    public required SLAYER_NoscopeConfig Config {get; set;}
    public void OnConfigParsed(SLAYER_NoscopeConfig config)
    {
        Config = config;
    }

    public bool[] g_Noscope= new bool[64];
    public bool[] g_Zoom = new bool[64];
    public bool adminNoscope = false;

    
    public override void Load(bool hotReload)
    {
        AddCommand("css_ns", "Enabled/Disabled Scope", cmd_ns);
        AddCommand("css_noscope", "Enabled/Disabled Scope", cmd_AdminNS);
        RegisterListener<Listeners.OnTick>(() =>
        {
            if(Config.PluginEnabled) // if Plugin is Enabled
            {
                foreach (var player in Utilities.GetPlayers().Where(player => player != null && player.IsValid && player.Connected == PlayerConnectedState.PlayerConnected && !player.IsHLTV && player.TeamNum > 1 && player.Pawn.Value!.LifeState == (byte)LifeState_t.LIFE_ALIVE))
                {
                    OnTick(player);
                }
            }
            
        });
    }
    
    private void cmd_ns(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(player == null) // if player is server then return
        {
            commandInfo.ReplyToCommand(Localizer["RCON.CannotUse"]);
            return;
        }
        if(Config.PluginEnabled == false)
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Plugin.Disabled"]}");
            return;
        }
        if(Config.PlayerCanUseNsCmd == false)
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Command.Disabled"]}");
            return;
        }
        if(Config.AlwaysDisableScope)
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Command.Unavailable"]}");
            return;
        }
        if(g_Noscope[player.Slot] == false)
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Player.NoScope.Enabled"]}");
            Server.PrintToChatAll($" {Localizer["Chat.Prefix"]} {Localizer["Player.NoScope.Enabled.All", player.PlayerName]}");
            g_Noscope[player.Slot] = true;
        }
        else
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Player.NoScope.Disabled"]}");
            Server.PrintToChatAll($" {Localizer["Chat.Prefix"]} {Localizer["Player.NoScope.Disabled.All", player.PlayerName]}");
            g_Noscope[player.Slot] = false;
        }
        
    }
    private void cmd_AdminNS(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(Config.PluginEnabled == false)
        {
            if(player != null)player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Plugin.Disabled"]}");
            else commandInfo.ReplyToCommand(Localizer["RCON.Plugin.Disabled"]);
            return;
        }
        if(player != null && !AdminManager.PlayerHasPermissions(player, Config.AdminFlagtoForceNS))
        {
            if(player != null)player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Admin.NoPermission"]}");
            return;
        }
        if(player != null && Config.AlwaysDisableScope)
        {
            player.PrintToChat($" {Localizer["Chat.Prefix"]} {Localizer["Command.Unavailable"]}");
            return;
        }
        if(adminNoscope == false)
        {
           adminNoscope = true;
            Server.PrintToChatAll($" {Localizer["Chat.Prefix"]} {Localizer["Admin.NoScope.Disabled"]}");
        }
        else
        {
            adminNoscope = false;
            Server.PrintToChatAll($" {Localizer["Chat.Prefix"]} {Localizer["Admin.NoScope.Enabled"]}");
        }
    }
    private void OnTick(CCSPlayerController? player)
    {
        if (!Config.PluginEnabled || player == null || !player.IsValid || player.PlayerPawn.Value == null)
            return;

        if(g_Noscope[player.Slot] || adminNoscope || Config.AlwaysDisableScope)
        {
            try
            {
                if(player.PlayerPawn.Value.WeaponServices!.MyWeapons.Count != 0)
                {
                    var ActiveWeaponName = player.PlayerPawn.Value.WeaponServices?.ActiveWeapon?.Value?.DesignerName;
                    if(!string.IsNullOrEmpty(ActiveWeaponName) && (ActiveWeaponName.Contains("weapon_ssg08") || ActiveWeaponName.Contains("weapon_awp")
                    || ActiveWeaponName.Contains("weapon_scar20") || ActiveWeaponName.Contains("weapon_g3sg1")))
                    {
                        player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value!.NextSecondaryAttackTick = Server.TickCount + 5000;
                        var buttons = player.Buttons;
                        if(!g_Zoom[player.Slot] && (buttons & PlayerButtons.Attack2) != 0)
                        {
                            g_Zoom[player.Slot] = true;
                            if(Config.ShowYouCantScopeMsg)
                            {
                                Server.NextFrame(() => {
                                    player.PrintToChat($"{Localizer["Chat.Prefix"]} {Localizer["Player.CantScope"]}");
                                });
                            }
                        }
                        else if(g_Zoom[player.Slot] && (buttons & PlayerButtons.Attack2) == 0)
                        {
                            g_Zoom[player.Slot] = false;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.LogWarning($"SLAYER Noscope Warning on OnTick: {ex}");
            }
        }
    }
    [GameEventHandler(HookMode.Pre)]
    public HookResult BulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        var player = @event.Userid;
        if (player == null || !player.IsValid || !Config.PluginEnabled || !Config.BulletTracers)
            return HookResult.Continue;
        if(g_Noscope[player.Slot] || adminNoscope || Config.AlwaysDisableScope)
        {
            if(player.PlayerPawn.Value!.WeaponServices!.MyWeapons.Count != 0)
            {
                 try
                {
                    var ActiveWeaponName = player.PlayerPawn.Value.WeaponServices?.ActiveWeapon?.Value!.DesignerName;
                    if(ActiveWeaponName != null && ActiveWeaponName != "" && ActiveWeaponName.Contains("weapon_ssg08") || ActiveWeaponName!.Contains("weapon_awp")
                    || ActiveWeaponName.Contains("weapon_scar20") || ActiveWeaponName.Contains("weapon_g3sg1"))
                    {
                        
                        Vector PlayerPosition = player.PlayerPawn.Value.AbsOrigin!;
                        Vector BulletOrigin = new Vector(PlayerPosition.X, PlayerPosition.Y, PlayerPosition.Z+64);
                        float[] bulletDestination = new float[3];
                        bulletDestination[0] = @event.X;
                        bulletDestination[1] = @event.Y;
                        bulletDestination[2] = @event.Z;
                        if(player.TeamNum == 3)DrawLaserBetween(player, BulletOrigin, new Vector(bulletDestination[0], bulletDestination[1], bulletDestination[2]), Color.Blue, 1.0f, 2.0f);
                        else if(player.TeamNum == 2)DrawLaserBetween(player, BulletOrigin, new Vector(bulletDestination[0], bulletDestination[1], bulletDestination[2]), Color.Red, 1.0f, 2.0f);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogWarning($"SLAYER Noscope Warning on BulletImpact: {ex}");
                }
            }
        }
        return HookResult.Continue;
    }
    
    public void DrawLaserBetween(CCSPlayerController player, Vector startPos, Vector endPos, Color color, float life, float width)
    {
        CBeam beam = Utilities.CreateEntityByName<CBeam>("beam")!;
        if (beam == null)
        {
            Logger.LogError($"Failed to create beam...");
            return;
        }
        beam.Render = color;
        beam.Width = width;
        
        beam.Teleport(startPos, player.PlayerPawn.Value!.AbsRotation, player.PlayerPawn.Value.AbsVelocity);
        beam.EndPos.X = endPos.X;
        beam.EndPos.Y = endPos.Y;
        beam.EndPos.Z = endPos.Z;
        beam.DispatchSpawn();
        AddTimer(life, () => { beam.Remove(); }); // destroy beam after specific time
    }
}

