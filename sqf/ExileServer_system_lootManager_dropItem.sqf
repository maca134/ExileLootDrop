/**
 * ExileServer_system_lootManager_dropItem
 *
 * maca134
 * www.maca134.co.uk
 * © 2016 maca134
 *
 * This work is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License. 
 * To view a copy of this license, visit http://creativecommons.org/licenses/by-nc-nd/4.0/.
 *
 * Examples:
 * Get single item (returns string so is backwards compatible with Exile): 
 * _item = 'table' call ExileServer_system_lootManager_dropItem;
 *
 * Get multiple items (returns an array of items, this is good for mission stuff):
 * _items = ['table', 10] call ExileServer_system_lootManager_dropItem;
 *
 */
 
private _input = _this;
if (typeName _input == 'STRING') then {
	_input = [_input, 1];
};
_input params [
	['_table', '', ['']],
	['_amount', 1, [0]]
];
private _packet = format['%1|%2', _table, _amount];
private _response = 'ExileLootDrop' callExtension _packet;
if ((_response select [0, 5]) == 'ERROR') exitWith {
	diag_log format['ExileLootDrop: Extension return error. Check logs! - %1 - %2', _packet, _response];
	""
};
private _return = if (_amount > 1) then {
	_response splitString '|'
} else {
	_response
};
_return