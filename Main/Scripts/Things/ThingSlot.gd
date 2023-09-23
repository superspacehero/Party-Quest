extends Node3D
class_name ThingSlot

@export var thing_type: String = ""
@export var disable_when_in_inventory: bool = false

var thing: GameThing = null

func add_thing(thing_to_add: GameThing, set_parent: bool = true) -> void:
	if !thing_to_add or thing_type == "" and (thing_type != thing_to_add.thing_type and thing_type != thing_to_add.thing_subtype):
		return

	thing = thing_to_add

	if set_parent:
		add_child(thing_to_add)

	thing_to_add.visible = !disable_when_in_inventory

func remove_thing(new_parent: Node3D = null) -> void:
	if !thing:
		return

	if !new_parent:
		new_parent = thing.parent

	new_parent.add_child(thing)
	thing.visible = true
	thing = null
