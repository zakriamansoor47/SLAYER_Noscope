# Donation
<a href="https://www.buymeacoffee.com/slayer47" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important;box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;-webkit-box-shadow: 0px 3px 2px 0px rgba(190, 190, 190, 0.5) !important;" ></a>

## Description:
This is my first plugin for [CounterStrikeSharp](https://docs.cssharp.dev/). This plugin disables Scope of Scope Weapons like AWP, scout, etc.

## Installation:
**1.** Upload files to your server.

**2.** Edit **configs/plugins/SLAYER_Noscope/SLAYER_Noscope.json** if you want to change the settings.

**3.** Change the Map **or** Restart the Server **or** Load the Plugin.

## Features:
**1.** Allow Players to Noscope by the player command `!ns`

**2.** Allow Admin (with specific Flag) to Force Players to Noscope.

**3.** Players Can't use the command to Enable/Disable Scope when Admin forces them to Noscope.

**4.** Show Text in Chat when Players/Admin Turn On/Off Scope

**5.** Support Bullet Tracers

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
  "BulletTracers": true,                // Enable/Disable Bullet Tracers. (false = Disabled, true = Enabled)
  "AdminFlagtoForceNS": "@css/root",    // Admin flag Which can Enable/Disable Scope of All Players by CMD (!noscope)
  "ConfigVersion": 1                    // Don't Change this
}
```
