extends Node

@export var scene_path: PackedScene = null
@export var load_on_start: bool = false
@export var load_wait_time: float = 0.0

func _ready():
    if load_on_start:
        if load_wait_time > 0.0:
            await get_tree().create_timer(load_wait_time).timeout
        load_scene()

func load_scene():
    get_tree().change_scene_to_packed(scene_path)