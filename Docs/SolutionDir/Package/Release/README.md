# XStorage

XStorage lets you open multiple chests at once, rename them, and move items/stacks to the most suitable chest.

<img src="https://raw.githubusercontent.com/SpikeHimself/XStorage/main/images/screenshot-v1.1.0-small.png" height="480" />


# Features

#### Name your chests

By pressing alt-interact (`Shift + E` by default), you can give your chests a name. This name is shown when you hover over the chest, and also in the panel above the inventory when a chest is opened.

#### Open many chests

To display multiple chests, XStorage creates a new panel in the UI. The panel will automatically expand to fit in as many chests as it can. You can restrict the width and height in XStorage's config.

When you open more chests than XStorage can display on the screen, this panel will become scrollable.

#### Moving items/stacks to the most suitable chest

When you auto-move an item from your inventory (via `Ctrl + Click`), XStorage tries to find the most suitable chest to put the item/stack in. It does so by picking the chest containing the highest quantity of the item you are moving, from all opened chests that still have free space.

`Ctrl + Click`-ing an item or stack in any of the chests will always make it go to the player inventory (equal to vanilla behaviour).

#### Multiplayer

In multiplayer games, all players need to have XStorage installed, or it will not work. If you play on a dedicated server, the server also has to have XStorage installed.

#### Mod compatibility

As of v1.1.3, XStorage can be stopped from opening nearby chests, if the chest being opened is marked with the XStorage "SkipMark". This allows other mods, such as [Seidr Chest](https://valheim.thunderstore.io/package/Neobotics/SeidrChest/), to function as intended.

To do this, set the `XStorage_SkipMark` to `true` in the Container's ZDO, like so:

```
container.m_nview.GetZDO().Set("XStorage_SkipMark", true);
```

If you need assistance with this, reach out to me on [XStorage's GitHub page](https://github.com/SpikeHimself/XStorage).

#### Known shortcomings

There are a few things that, at this stage, XStorage does not do well, or at all:

* **Gamepad input**: XStorage does not deal with gamepad input at all. That is to say, the UI kind of breaks when you use a gamepad. I will look into this in the future, but for now this mod is pretty much mouse/keyboard only.


XStorage looks best when playing at a 16:9 ratio with a UI scaling of 95% or smaller.


# Configuration

XStorage's config file, which can be found at `Valheim\BepInEx\config\yay.spikehimself.xstorage.cfg`, contains the following settings:

`NearbyChestRange`

The radius in meters within which to look for nearby chests. Setting this too high might cause performance issues!

`MaxOpenChests`

The maximum amount of chests to open at once. 0 or fewer means infinite.

`Panel Position`

The Panel Position section contains the saved screen positions of each panel (per panel size). It is not recommended to edit these values manually.

`MaxColumns` and `MaxRows`

The maximum amount of rows and columns that XStorage can expand the containers panel to.

`PanelScale`

The relative size of XStorage's panel. Can be any value between 0.5 and 1.5, where 0.5 = 50%, 1 = 100%, and 1.5 = 150%


# Installation instructions

XStorage is a [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) plugin. As such, you must have BepInEx installed. Most other Valheim mods are also BepInEx plugins, so chances are you already have this.

XStorage makes use of the [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/) library, so you must install that before installing XStorage. If you do not install Jotunn, XStorage will simply not be loaded by your game and it will not work.

I very strongly recommend using a mod manager such as [Vortex](https://www.nexusmods.com/site/mods/1) or [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/). They will take care of everything for you and you don't have to worry about which files go where. I recommend against manual installation.
1. Make sure you have [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) installed.
2. Install [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/).
3. On [Nexus Mods](https://www.nexusmods.com/valheim/mods/2290) click 'Mod manager download', or on [Thunderstore](https://valheim.thunderstore.io/package/SpikeHimself/XStorage/) click 'Install with Mod Manager'.


To install XStorage on a dedicated server, copy all of the contents of the `plugins\` directory found inside the .zip file download to the  `Valheim\BepInEx\plugins\` directory on your server. 


# Bugs and Feature Requests

To report a bug, please navigate to the [Issues page](https://github.com/SpikeHimself/XStorage/issues), click [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose), choose `Bug report`, and fill out the template.

For feature requests, choose `Feature request` on the [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose) page.


# I did more too!

Please have a look at my other mod too! [XPortal](https://valheim.thunderstore.io/package/SpikeHimself/XPortal/) lets you select a portal destination from a list of existing portals, so that you don't have to match portal tags anymore.

