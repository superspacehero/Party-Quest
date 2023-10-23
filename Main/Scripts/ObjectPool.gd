extends Node
class_name ObjectPool

@export var object_scene : PackedScene
var object_pool : Array = []

func _init(scene_path: PackedScene, parent: Node3D):
    self.object_scene = scene_path
    self.object_parent = parent

func get_object_from_pool(object_position: Vector3) -> Node: 
    var component = null
    for pooled_object in object_pool:
        if not pooled_object.is_inside_tree():
            component = pooled_object
            break

    if component == null:
        var obj = object_scene.instance()
        self.add_child(obj)
        obj.global_position = object_position
        object_pool.append(obj)
        component = obj

    component.global_position = object_position

    component.visible = true
    component.process_mode = Node.PROCESS_MODE_INHERIT

    return component

func return_object_to_pool(obj: Node):
    obj.visible = false
    obj.process_mode = Node.PROCESS_MODE_DISABLED
