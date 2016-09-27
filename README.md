# Exile Loot Drop - [Download](https://github.com/maca134/ExileLootDrop/releases)

A server mod/extension to replace the Exile loot drop function with a dll.

Examples:

Get single item (returns string so is backwards compatible with Exile): 
```
_item = 'table' call ExileServer_system_lootManager_dropItem;
```

Get multiple items (returns an array of items, this is good for mission stuff):
```
_items = ['table', 10] call ExileServer_system_lootManager_dropItem;
```

To Install:
Run the mod on the server and stick the below into CfgExileCustomCode in you mission files
```
class CfgExileCustomCode
{
	...
	ExileServer_system_lootManager_dropItem = "\ExileLootDrop\ExileServer_system_lootManager_dropItem.sqf";
	ExileServer_system_lootManager_spawnLootInBuilding = "\ExileLootDrop\ExileServer_system_lootManager_spawnLootInBuilding.sqf";
	...
};
```
Now replace @ExileLootDrop\ExileLootDrop.cfg with your own servers loot (this is the default Exile table). The loot is in the "pre-compiled" format.

### Performance

ExileLootDrop.VR mission contains the original Exile method for loot and the original tables for testing
```
// SQF
'CivillianLowerClass' call ExileServer_system_lootManager_dropItem_sqf

// DLL
'CivillianLowerClass' call ExileServer_system_lootManager_dropItem_ext
```

![Performance](https://dl.dropboxusercontent.com/s/wpk6m54pivmk04g/9964d7ad-7f4f-46bf-b5c5-5c64b696f8c3.png)

### Tester EXE

![Performance1](https://dl.dropboxusercontent.com/s/8kxb4uhlbv362mu/abd035c5-009b-4c3f-849e-525862c8f09e.png)