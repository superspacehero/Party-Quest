extends CharacterPartThing
class_name BodyThing

@export var collider_dimensions: Vector2 = Vector2(1, 2)

func get_thing_subtype() -> String:
    return "Body"
