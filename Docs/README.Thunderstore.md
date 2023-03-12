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

#### Known shortcomings

There are a few things that, at this stage, XStorage does not do well, or at all:
* **Gamepad input**: XStorage does not deal with gamepad input at all. That is to say, the UI kind of breaks when you use a gamepad. I will look into this in the future, but for now this mod is pretty much mouse/keyboard only.


XStorage looks best when playing at a 16:9 ratio with a UI scaling of 95% or smaller.


# Installation instructions

XStorage is a [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) plugin. As such, you must have BepInEx installed. Most other Valheim mods are also BepInEx plugins, so chances are you already have this.

XStorage makes use of the [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/) library, so you must install that before installing XStorage. If you do not install Jotunn, XStorage will simply not be loaded by your game and it will not work.

I very strongly recommend using a mod manager such as [Vortex](https://www.nexusmods.com/site/mods/1) or [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/). They will take care of everything for you and you don't have to worry about which files go where. I recommend against manual installation.
1. Make sure you have [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) installed.
2. Install [Jotunn](https://valheim.thunderstore.io/package/ValheimModding/Jotunn/).
3. On [Nexus Mods](https://www.nexusmods.com/valheim/mods/2290) click 'Mod manager download', or on [Thunderstore](https://valheim.thunderstore.io/package/SpikeHimself/XStorage/) click 'Install with Mod Manager'.



# Changelogs

* **v1.1.0** (2023-03-11)

	* Fix error when placing a new chest

	* Fix a HarmonyX warning that occurs when loading XStorage; this was caused by a library I use locally to manage documentation, but XStorage does not ship with this library.

	* Fix being able to rename chests that are protected by a ward

	* UI overhaul: 
	
		* You can now drag the XStorage panel 

		* You can set the maximum panel size in XStorage's config file using MaxColumns and MaxRows. Default value is 2 columns by 3 rows. XStorage will still restrict the size by what fits on your screen.

		* XStorage will store the position of the panel per grid size when you close the panel, so that next time you open a panel of the same size, it will be restored to that position on the screen.

	* Many code improvements and optimisations

<details>
<summary>Click to view previous versions</summary>

* **v1.0.2** (2023-03-02)

	* Fix tooltips not always being fully visible

	* Fix tooltips sometimes escaping the mouse pointer

	* Reworked a large portion of the containers panel UI

* **v1.0.1** (2023-02-28)

	* Remove "valheim.exe" check as it stops dedicated servers from loading the mod.

* **v1.0.0** (2023-02-28)

	* Initial release

</details>


# Bugs and Feature Requests

To report a bug, please navigate to the [Issues page](https://github.com/SpikeHimself/XStorage/issues), click [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose), choose `Bug report`, and fill out the template.

For feature requests, choose `Feature request` on the [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose) page.


# I did more too!

Please have a look at my other mod too! [XPortal](https://valheim.thunderstore.io/package/SpikeHimself/XPortal/) lets you select a portal destination from a list of existing portals, so that you don't have to match portal tags anymore.


# Support me

My mods are free and will remain free, for everyone to use, edit or learn from. I lovingly poured many hours of hard work into these projects. If you enjoy my mods and want to support my work, don't forget to click the Like button, and please consider buying me a coffee :)

[<img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" height="40" align="right" />](https://www.buymeacoffee.com/SpikeHimself)
