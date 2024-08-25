extends Menu
class_name SceneLoadMenu

@export var scene_to_load: PackedScene

func select():
	await super.select()

	if scene_to_load != null:
		get_tree().change_scene_to_packed(scene_to_load)