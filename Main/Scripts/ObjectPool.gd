extends Node
class_name ObjectPool

@export var object_scene : PackedScene
@export var object_parent : Node
var object_pool : Array = []

func _init():
    if object_parent == null:
        object_parent = self

func get_object_from_pool(object_position: Vector3) -> Node: 
    var component = null
    for pooled_object in object_pool:
        if pooled_object.process_mode == Node.PROCESS_MODE_DISABLED:
            component = pooled_object
            break

    if component == null:
        var obj = object_scene.instantiate()
        object_parent.add_child(obj)  # Instead of self.add_child(obj)
        obj.global_position = object_position
        object_pool.append(obj)
        component = obj

    component.global_position = object_position

    component.visible = true
    component.process_mode = Node.PROCESS_MODE_INHERIT
    component._ready()

    return component

func return_object_to_pool(obj: Node):
    # Set the object to be outside the tree
    
    obj.visible = false
    obj.process_mode = Node.PROCESS_MODE_DISABLED
