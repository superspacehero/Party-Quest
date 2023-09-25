extends CharacterPartThing
class_name BodyThing

@export var collider_dimensions: Vector2 = Vector2(1, 2)

func _init():
	thing_subtype = "Body"
