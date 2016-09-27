Exile Loot Drop

A server mod/extension to replace the Exile loot drop function with a dll.

Examples:
Get single item (returns string so is backwards compatible with Exile): 
_item = 'table' call ExileServer_system_lootManager_dropItem;

Get multiple items (returns an array of items, this is good for mission stuff):
_items = ['table', 10] call ExileServer_system_lootManager_dropItem;

ExileLootDrop.VR mission contains the original Exile method for loot and the original tables for testing

// SQF
'CivillianLowerClass' call ExileServer_system_lootManager_dropItem_sqf

// DLL
'CivillianLowerClass' call ExileServer_system_lootManager_dropItem_ext

To Install:
Run the mod on the server and stick the below into CfgExileCustomCode in you mission files
```
class CfgExileCustomCode
{
	...
	ExileServer_system_lootManager_dropItem = "\ExileLootDrop\ExileServer_system_lootManager_dropItem.sqf";
	...
};
```