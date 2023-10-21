extends Node3D
class_name ThingSlot

@export var thing_type: String = ""
@export var disable_when_in_inventory: bool = false
@export var sorting_offset: int = 0

var thing: Node3D = null

func add_thing(thing_to_add: GameThing, set_parent: bool = true) -> void:
	if !thing_to_add or thing_type == "" and (thing_type != thing_to_add.get_thing_type() and thing_type != thing_to_add.get_thing_subtype()):
		return

	thing = thing_to_add.thing_root

	if set_parent:
		add_child(thing)

	thing.visible = !disable_when_in_inventory

func remove_thing(new_parent: Node3D = null) -> void:
	if !thing:
		return

	if !new_parent:
		new_parent = thing.parent

	new_parent.add_child(thing)
	thing.visible = true
	thing = null
