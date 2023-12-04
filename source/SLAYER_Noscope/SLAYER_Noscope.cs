using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Entities;


namespace SLAYER_Noscope;

#pragma warning disable CS8602 // Dereference of a possibly null reference. Used this to remove compile warnings

public class ConfigSpecials : BasePluginConfig
{
    [JsonPropertyName("PluginEnabled")] public bool PluginEnabled { get; set; } = true;
    [JsonPropertyName("AlwaysDisableScope")] public bool AlwaysDisableScope { get; set; } = false;
    [JsonPropertyName("PlayerCanUseNsCmd")] public bool PlayerCanUseNsCmd { get; set; } = true;
    [JsonPropertyName("ShowYouCantScopeMsg")] public bool ShowYouCantScopeMsg { get; set; } = true;
    [JsonPropertyName("UseRemovingWeaponMethod")] public bool UseRemovingWeaponMethod { get; set; } = false;
    [JsonPropertyName("AdminFlagtoForceNS")] public string AdminFlagtoForceNS { get; set; } = "@css/root";
}

[MinimumApiVersion(80)]
public class SLAYER_Noscope : BasePlugin, IPluginConfig<ConfigSpecials>
{
    public override string ModuleName => "SLAYER_Noscope";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "SLAYER";
    public override string ModuleDescription => "Enable/Disable Noscope for all Scope Weapons";


    public required ConfigSpecials Config {get; set;}

    public bool[] g_Noscope= new bool[64];
    public bool adminNoscope = false;

    public void OnConfigParsed(ConfigSpecials config)
    {
        Config = config;
    }
    public override void Load(bool hotReload)
    {
        AddCommand("css_ns", "Enabled/Disabled Scope", cmd_ns);
        AddCommand("css_noscope", "Enabled/Disabled Scope", cmd_AdminNS);
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
    [GameEventHandler(HookMode.Pre)]
    public HookResult EventNoscope(EventWeaponZoom @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        if(player == null || !player.Pawn.IsValid)
            return HookResult.Continue;
        if(player != null && g_Noscope[player.Slot] || adminNoscope || Config.AlwaysDisableScope)
        {
            
            if(Config.UseRemovingWeaponMethod)
            {
                var CurrentWeaponName = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.DesignerName;
                var SavedAmmoClip = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.Clip1;
                player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.Remove();
                player.GiveNamedItem(CurrentWeaponName);
            }
            else 
            {
                if(CheckIsHaveWeapon("bayonet", player) == false && CheckIsHaveWeapon("knife", player) == false) // If knife not found
                {
                    player.GiveNamedItem("weapon_knife"); // Give Knife
                    player.ExecuteClientCommand("slot3");// Change Weapon to knife
                }
                else // if knife found
                {
                    player.ExecuteClientCommand("slot3");// Change Weapon to knife
                    
                }
            }
            //int  lol = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.NextSecondaryAttackTick;
            //player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value.NextSecondaryAttackTick = Convert.ToInt32(Server.CurrentTime+1.0);
            //CBasePlayerWeapon weapon = player.PlayerPawn.Value.WeaponServices!.ActiveWeapon.Value;
            //SchemaString<CBasePlayerWeapon> playernextzoom = new SchemaString<CBasePlayerWeapon>(weapon, "m_nNextSecondaryAttackTick");
            //playernextzoom.Set(Convert.ToString(Server.CurrentTime+50.0));
            if(Config.ShowYouCantScopeMsg)
            {
                Server.NextFrame(() => {
                    player.PrintToChat($"{ChatColors.Lime}[{ChatColors.Darkred}No{ChatColors.Green}Scope{ChatColors.Lime}] {ChatColors.LightPurple}You {ChatColors.Darkred}can't {ChatColors.Lime}Scope!");
                });
            }
            
        }
        
        return HookResult.Continue;
    }
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
    }
    /*
    public class SchemaString<SchemaClass> : NativeObject where SchemaClass : NativeObject
    {
        public SchemaString(SchemaClass instance, string member) : base(Schema.GetSchemaValue<nint>(instance.Handle, typeof(SchemaClass).Name!, member))
        { }

        public unsafe void Set(string str)
        {
            byte[] bytes = this.GetStringBytes(str);

            for (int i = 0; i < bytes.Length; i++)
            {
                Unsafe.Write((void*)(this.Handle.ToInt64() + i), bytes[i]);
            }

            Unsafe.Write((void*)(this.Handle.ToInt64() + bytes.Length), 0);
        }
        private byte[] GetStringBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
    }*/
}

