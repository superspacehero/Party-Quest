@tool
extends Node3D
class_name LightingSetter

@export var skybox_mesh: MeshInstance3D:
	set(value):
		skybox_mesh = value
		if skybox_mesh:
			if Engine.is_editor_hint():
				skybox_material = skybox_mesh.get_active_material(0)
			else:
				skybox_material = skybox_mesh.get_active_material(0).duplicate()

			skybox_mesh.set_material_override(skybox_material)

			set_lighting_value()
@export var variable_name: String = "sky_time"
@export_range(0.0, 1.0) var sky_time: float = 0.5:
	set(value):
		sky_time = value
		if skybox_material:
			skybox_material.set_shader_parameter(variable_name, sky_time)

var skybox_material = null
var previous_rotation: Vector3 = Vector3.ZERO

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if previous_rotation != rotation:
		set_lighting_value()

func set_lighting_value() -> void:
	if skybox_material:
		# Convert the rotation to a value between 0 and 1
		sky_time = transform.basis.z.normalized().angle_to(Vector3.UP) / PI
		previous_rotation = rotation