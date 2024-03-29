# XStorage

XStorage is a Valheim mod that lets you open multiple chests at once, rename them, and move items/stacks to the most suitable chest.

<img src="https://raw.githubusercontent.com/SpikeHimself/XStorage/main/images/screenshot-v1.1.0-small.png" height="480" />

# Where to download
(click the image!)

<div align="center">

&nbsp;

[![Nexus Mods](https://raw.githubusercontent.com/SpikeHimself/resources/main/images/thirdparty/nexus-logo-small.png)](https://www.nexusmods.com/valheim/mods/2290) &nbsp;&nbsp;&nbsp; [![Thunderstore](https://raw.githubusercontent.com/SpikeHimself/resources/main/images/thirdparty/thunderstore-logo-small.png)](https://valheim.thunderstore.io/package/SpikeHimself/XStorage/)

&nbsp;

</div>


# Download and installation instructions (for players)

XStorage is a [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) plugin. As such, you must have BepInEx installed. Most other Valheim mods are also BepInEx plugins, so chances are you already have this.

XStorage makes use of the [Jotunn](https://www.nexusmods.com/valheim/mods/1138) library, so you must install that before installing XStorage. If you do not install Jotunn, XStorage will simply not be loaded by your game and it will not work.

I very strongly recommend using a mod manager such as [Vortex](https://www.nexusmods.com/site/mods/1) or [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/). They will take care of everything for you and you don't have to worry about which files go where. I recommend against manual installation.
1. Make sure you have [BepInEx](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/) installed.
2. Install [Jotunn](https://www.nexusmods.com/valheim/mods/1138).
3. On [Nexus Mods](https://www.nexusmods.com/valheim/mods/2290) click 'Mod manager download', or on [Thunderstore](https://valheim.thunderstore.io/package/SpikeHimself/XStorage/) click 'Install with Mod Manager'.


To install XStorage on a dedicated server, copy all of the contents of the `plugins\` directory found inside the .zip file download to the  `Valheim\BepInEx\plugins\` directory on your server. 


# Bugs and Feature Requests

To report a bug, please navigate to the [Issues page](https://github.com/SpikeHimself/XStorage/issues), click [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose), choose `Bug report`, and fill out the template.

For feature requests, choose `Feature request` on the [New issue](https://github.com/SpikeHimself/XStorage/issues/new/choose) page.


# Installation instructions (for developers)

I will soon write a guide to get XStorage working in your development environment. For now, you can probably figure some stuff out by having a look at the [JotunnModStub](https://github.com/Valheim-Modding/JotunnModStub) project that XStorage is based on. Please bear in mind that the information there might have changed since XStorage was created, and that XStorage itself may over time have diverted from the steps laid out there. Again, a guide will follow soon!


# I did more too!

Please have a look at my other mod too! [XPortal](https://www.nexusmods.com/valheim/mods/2239) lets you select a portal destination from a list of existing portals, so that you don't have to match portal tags anymore.

