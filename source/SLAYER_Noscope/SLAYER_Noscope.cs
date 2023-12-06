using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Logging;
using System.Text.Json.Serialization;
using System.Drawing;
using Microsoft.Extensions.Logging;


namespace SLAYER_Noscope;

// Used these to remove compile warnings
#pragma warning disable CS8600
#pragma warning disable CS8602 
#pragma warning disable CS8604

public class ConfigSpecials : BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
    [JsonPropertyName("AlwaysDisableScope")] public bool AlwaysDisableScope { get; set; } = false;
    [JsonPropertyName("PlayerCanUseNsCmd")] public bool PlayerCanUseNsCmd { get; set; } = true;
    [JsonPropertyName("ShowYouCantScopeMsg")] public bool ShowYouCantScopeMsg { get; set; } = true;
    [JsonPropertyName("BulletTracers")] public bool BulletTracers { get; set; } = true;
    [JsonPropertyName("AdminFlagtoForceNS")] public string AdminFlagtoForceNS { get; set; } = "@css/root";
}

public class SLAYER_Noscope : BasePlugin, IPluginConfig<ConfigSpecials>
{
    public override string ModuleName => "SLAYER_Noscope";
    public override string ModuleVersion => "1.1";
    public override string ModuleAuthor => "SLAYER";
    public override string ModuleDescription => "Enable/Disable Noscope for all Scope Weapons";


    public required ConfigSpecials Config {get; set;}

    public bool[] g_Noscope= new bool[64];
    public bool[] g_Zoom = new bool[64];
    public bool adminNoscope = false;

    public void OnConfigParsed(ConfigSpecials config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        AddCommand("css_ns", "Enabled/Disabled Scope", cmd_ns);
        AddCommand("css_noscope", "Enabled/Disabled Scope", cmd_AdminNS);
        RegisterListener<Listeners.OnTick>(() =>
        {
            if(Config.PluginEnabled) // if Plugin is Enabled
            {
                foreach (var player in Utilities.GetPlayers().Where(player => player is { IsValid: true, PawnIsAlive: true }))
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
            commandInfo.ReplyToCommand("[NoScope] Cannot use command from RCON");
            return;
        }
        if(Config.PluginEnabled == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}Plugin is Disabled!");
            return;
        }
        if(Config.PlayerCanUseNsCmd == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}Command is Disabled!");
            return;
        }
        if(Config.AlwaysDisableScope)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}Command is Unavailable Right Now!");
            return;
        }
        if(g_Noscope[player.Slot] == false)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Purple}Now {ChatColors.LightPurple}you {ChatColors.Darkred}can't {ChatColors.Lime}Scope!");
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Green}{player.PlayerName} {ChatColors.Purple}now {ChatColors.Darkred}can't {ChatColors.Lime}Scope!");
            g_Noscope[player.Slot] = true;
        }
        else
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Purple}Now {ChatColors.LightPurple}you {ChatColors.Darkred}can {ChatColors.Lime}Scope!");
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Green}{player.PlayerName} {ChatColors.Purple}now {ChatColors.Darkred}can {ChatColors.Lime}Scope!");
            g_Noscope[player.Slot] = false;
        }
        
    }
    private void cmd_AdminNS(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if(Config.PluginEnabled == false)
        {
            if(player != null)player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}Plugin is Disabled!");
            else commandInfo.ReplyToCommand("[NoScope] Plugin is Disabled!");
            return;
        }
        if(player != null && !AdminManager.PlayerHasPermissions(player, Config.AdminFlagtoForceNS))
        {
            if(player != null)player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}You don't have permission to use this command!");
            return;
        }
        if(player != null && Config.AlwaysDisableScope)
        {
            player.PrintToChat($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Darkred}Command is Unavailable Right Now!");
            return;
        }
        if(adminNoscope == false)
        {
           adminNoscope = true;
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Purple}Admin {ChatColors.Darkred}Disabled {ChatColors.Lime}Scope!");
        }
        else
        {
            adminNoscope = false;
            Server.PrintToChatAll($" {ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.Purple}Admin {ChatColors.Darkred}Enabled {ChatColors.Lime}Scope!");
        }
    }
    private void OnTick(CCSPlayerController player)
    {
        if (player.Pawn == null || !player.Pawn.IsValid || !Config.PluginEnabled)
            return;

        if(g_Noscope[player.Slot] || adminNoscope || Config.AlwaysDisableScope)
        {
            var ActiveWeaponName = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.DesignerName;
            if(ActiveWeaponName.Contains("weapon_ssg08") || ActiveWeaponName.Contains("weapon_awp")
            || ActiveWeaponName.Contains("weapon_scar20") || ActiveWeaponName.Contains("weapon_g3sg1"))
            {
                player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.NextSecondaryAttackTick = Server.TickCount + 500;
                var buttons = player.Buttons;
                if(!g_Zoom[player.Slot] && (buttons & PlayerButtons.Attack2) != 0)
                {
                    g_Zoom[player.Slot] = true;
                    if(Config.ShowYouCantScopeMsg)
                    {
                        Server.NextFrame(() => {
                            player.PrintToChat($"{ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.LightPurple}You {ChatColors.Darkred}can't {ChatColors.Lime}Scope!");
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
    [GameEventHandler(HookMode.Pre)]
    public HookResult BulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        if (player.Pawn == null || !player.Pawn.IsValid || !Config.PluginEnabled || !Config.BulletTracers)
            return HookResult.Continue;
        if(g_Noscope[player.Slot] || adminNoscope || Config.AlwaysDisableScope)
        {
            var ActiveWeaponName = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.DesignerName;
            if(ActiveWeaponName.Contains("weapon_ssg08") || ActiveWeaponName.Contains("weapon_awp")
            || ActiveWeaponName.Contains("weapon_scar20") || ActiveWeaponName.Contains("weapon_g3sg1"))
            {
                
                Vector PlayerPosition = player.Pawn.Value.AbsOrigin;
                Vector BulletOrigin = new Vector(PlayerPosition.X, PlayerPosition.Y, PlayerPosition.Z+64);//bulletOrigin.X += 50.0f;
                float[] bulletDestination = new float[3];
                bulletDestination[0] = @event.X;
                bulletDestination[1] = @event.Y;
                bulletDestination[2] = @event.Z;
                if(player.TeamNum == 3)DrawLaserBetween(player, BulletOrigin, new Vector(bulletDestination[0], bulletDestination[1], bulletDestination[2]), Color.Blue, 1.0f, 2.0f);
                else if(player.TeamNum == 2)DrawLaserBetween(player, BulletOrigin, new Vector(bulletDestination[0], bulletDestination[1], bulletDestination[2]), Color.Red, 1.0f, 2.0f);
            }
        }
        return HookResult.Continue;
    }
    
    public void DrawLaserBetween(CCSPlayerController player, Vector startPos, Vector endPos, Color color, float life, float width)
    {
        CBeam beam = Utilities.CreateEntityByName<CBeam>("beam");
        if (beam == null)
        {
            Logger.LogError($"Failed to create beam...");
            return;
        }
        beam.Render = color;
        beam.Width = width;
        
        beam.Teleport(startPos, player.PlayerPawn.Value.AbsRotation, player.PlayerPawn.Value.AbsVelocity);
        beam.EndPos.X = endPos.X;
        beam.EndPos.Y = endPos.Y;
        beam.EndPos.Z = endPos.Z;
        beam.DispatchSpawn();
        AddTimer(life, () => { beam.Remove(); }); // destroy after 1s
    }
    /*
    private bool CheckIsHaveWeapon(string weapon_name, CCSPlayerController? pc)
    {
        
        foreach (var weapon in pc.PlayerPawn.Value.WeaponServices!.MyWeapons)
        {
            if (weapon is { IsValid: true, Value.IsValid: true })
            {
                if (weapon.Value.DesignerName.Contains($"{weapon_name}"))
                {
                    return true;
                }
            }
        }
        return false;
    }*/
}

