extends GameThing
class_name SubThing

@export var parent_thing: GameThing = null

@export var nodes_to_follow: Array = [Node3D]

func get_health():
	return parent_thing.get_health()

func set_health(value, relative=false):
	parent_thing.set_health(value, relative)