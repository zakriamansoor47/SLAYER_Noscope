# Donate: If you like my work, you can donate to me via [Steam Trade Offer](https://bit.ly/3qDpgPd)

## Description:
This is my first plugin for [CounterStrikeSharp](https://docs.cssharp.dev/). This plugin disables Scope of Scope Weapons like AWP, scout, etc.

## Features:
**1.** Upload files to your server.
**2.** Edit **configs/plugins/SLAYER_Noscope/SLAYER_Noscope.json** if you want to change the settings.
**3.** Change the Map **or** Restart the Server **or** Load the Plugin.

## Installation:
**1.** Allow Players to Noscope by the player command `!ns`
**2.** Allow Admin (with specific Flag) to Force Players to Noscope.
**3.** Players Can't use the command to Enable/Disable Scope when Admin forces them to Noscope.
**4.** Show Text in Chat when Players/Admin Turn On/Off Scope
**5.** There are **2 Methods** to disable scope. **First,** remove and give a new weapon when the player tries to Scope. **Second,** by switching the player's weapon to a knife when the player tries to Scope. You can change the Noscope Method from the **Configuration** file. 

## Requirements:
CounterStrikeSharp v80+

## Commands:
```
!ns - For Everyone to Enable/Disable Scope on themselves (Scope will always be disabled if "AlwaysDisableScope" is true in the config file)
!noscope - For Root Admin to Enable/Disable Scope for all Players (Scope will always disabled if "AlwaysDisableScope" is true in the config file)
css_noscope - For RCON (Console) to Enable/Disable Scope for all Players (Scope will always disabled if "AlwaysDisableScope" is true in the config file)
```

## Configuration:
```
{
  "PluginEnabled": true,                // Enable/Disable Noscope plugin. (false = Disabled, true = Enabled)
  "AlwaysDisableScope": false,          // Always Disable Scope in the Server. (false = No, true = Yes)
  "PlayerCanUseNsCmd": true,            // Players Can use Noscope Command (!ns) to Enable/Disable Scope on themselves (0 = No, 1 = Yes)
  "ShowYouCantScopeMsg": true,          // Show Message When player try to Scope (0 = No, 1 = Yes)
  "UseRemovingWeaponMethod": false,     // Method of Noscope (true = remove and give a new weapon when the player tries to Scope | false = by switching the player's weapon to a knife when                                            // the player tries to Scope.)
  "AdminFlagtoForceNS": "@css/root",    // Admin flag Which can Enable/Disable Scope of All Players by CMD (!noscope)
  "ConfigVersion": 1
}
```
