@tool
extends CharacterPartThing
class_name FacialPartThing

@export var face_frames : Vector2i = Vector2i(4, 3):
	set(value):
		face_mesh.get_surface_override_material(0).Frames = value
		face_frames = value

@export var face_offset : Vector2i = Vector2i(0, 0):
	set(value):
		face_mesh.get_surface_override_material(0).Offset = value
		face_offset = value

@export var face_mesh : MeshInstance3D:
	get:
		return get_parent().get_child(0)