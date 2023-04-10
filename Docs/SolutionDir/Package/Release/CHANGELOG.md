#### v1.1.4 (2023-04-10)

* No changes yet

<details>
<summary>Click to view previous versions</summary>

* **v1.1.3** (2023-04-10)

	* Don't open nearby containers when SkipMark is set (by other mods)

	* Clean up panel position values in config file

	* Dependency updates: BepInEx 5.4.2102, Jotunn 2.11.3

* **v1.1.2** (2023-03-15)

	* Dependency updates: BepInEx 5.4.21, Jotunn 2.11.0

	* Fix panel size calculation

	* Save panel positions as Vector2 instead of Vector3

* **v1.1.1** (2023-03-13)

	* Fix mod breaking after logging out and then starting/joining another game

	* Store panel positions in config instead of hidden away in player preferences

	* Clamp panel to screen boundaries after restoring its position

	* Fix error that sometimes appears when logging out or quitting the game

	* Fix error that can occur when `Ctrl+Click`-ing an empty cell in one of the the inventory grids

	* Use the position of the chest you're opening, instead of the position of your character, when searching for nearby chests

	* Order chests by weight so empty ones show up last

	* Add config option `NearbyChestRadius` to set how far away to look for chests for

	* Fix a compatibility issue whereby the Take All button would stop working if another mod added more buttons to the container panel

* **v1.1.0** (2023-03-11)

	* Fix error when placing a new chest

	* Fix a HarmonyX warning that occurs when loading XStorage; this was caused by a library I use locally to manage documentation, but XStorage does not ship with this library.

	* Fix being able to rename chests that are protected by a ward

	* UI overhaul: 
	
		* You can now drag the XStorage panel 

		* You can set the maximum panel size in XStorage's config file using MaxColumns and MaxRows. Default value is 2 columns by 3 rows. XStorage will still restrict the size by what fits on your screen.

		* XStorage will store the position of the panel per grid size when you close the panel, so that next time you open a panel of the same size, it will be restored to that position on the screen.

	* Many code improvements and optimisations

* **v1.0.2** (2023-03-02)

	* Fix tooltips not always being fully visible

	* Fix tooltips sometimes escaping the mouse pointer

	* Reworked a large portion of the containers panel UI

* **v1.0.1** (2023-02-28)

	* Remove "valheim.exe" check as it stops dedicated servers from loading the mod.

* **v1.0.0** (2023-02-28)

	* Initial release

</details>


