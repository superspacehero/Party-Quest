extends CharacterPartThing
class_name BodyThing

@export var collider_dimensions: Vector2 = Vector2(1, 2)

var animation_player: AnimationPlayer:
    get:
        return $AnimationPlayer

func get_thing_subtype() -> String:
    return "Body"
