@tool
extends CharacterPartThing
class_name FacialPartThing

@export var face_frames : Vector2i = Vector2i(4, 3):
	set(value):
		face_frames = value
		face_mesh.get_surface_override_material(0).set_shader_parameter("Frames", Vector2(face_frames.x, face_frames.y))

@export var face_offset : Vector2i = Vector2i(0, 0):
	set(value):
		face_offset = Vector2i(clampi(value.x, 0, face_frames.x - 1), clampi(value.y, 0, face_frames.y - 1))
		face_mesh.get_surface_override_material(0).set_shader_parameter("Offset", Vector2(face_offset.x, face_offset.y))

@export var face_mesh : MeshInstance3D:
	get:
		if face_mesh == null:
			return get_parent().get_child(0)
		return face_mesh