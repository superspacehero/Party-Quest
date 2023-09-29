extends Node3D
class_name GameplayCamera

# Singleton
static var instance: GameplayCamera = null

# Camera Variables
@export var camera_adjust_time = 0.25
@export var camera_sensitivity = 1.5

var camera_rotation_amount: Vector2 = Vector2.ZERO
@export var camera_offset = Vector3(0, 0, 7)
@export var camera_rotation = Vector3(22.5, 0, 0)
@export var rotation_limits = Vector2(-60, 60)
var current_rotation = Vector2.ZERO

@export var camera_object: GameThing = null
@export var camera_offset_node: Node3D = null

# Interpolation related variables
var interpolating = false
var target_position = Vector3.ZERO
var start_position = Vector3.ZERO
var elapsed_time = 0.0

func _ready():
	if instance != null and instance != self:
		instance.queue_free()
	instance = self

	camera_offset_node.position = camera_offset
	# Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _process(delta):
	if camera_rotation_amount.length() > 0:
		if (camera_rotation_amount.length() > 1):
			camera_rotation_amount = camera_rotation_amount.normalized()
		rotate_camera(camera_rotation_amount)

	if interpolating:
		center_camera_interpolation(delta)

# Rotate the camera based on mouse input
func rotate_camera(relative: Vector2):
	current_rotation.x += relative.x * camera_sensitivity
	current_rotation.y += relative.y * camera_sensitivity
	current_rotation.x = clamp(current_rotation.x, rotation_limits.x, rotation_limits.y)
	self.rotation_degrees.x = -current_rotation.x
	self.rotation_degrees.y = -current_rotation.y

# Set the camera's attachment to a GameThing object
func set_camera_object(game_thing: GameThing, camera_height: float = 0.5, immediate: bool = false):
	if self.get_parent() != game_thing.thing_root:
		self.reparent(game_thing.thing_root)
	start_position = self.global_position
	target_position = lerp(game_thing.thing_root.global_position, game_thing.thing_top.global_position, camera_height)

	if immediate:
		self.global_position = target_position
		elapsed_time = camera_adjust_time
	else:
		elapsed_time = 0.0
		interpolating = true

# Handle the interpolation of the camera towards the target
func center_camera_interpolation(delta):
	elapsed_time += delta
	if elapsed_time < camera_adjust_time:
		var ratio = elapsed_time / camera_adjust_time
		self.global_position = start_position.lerp(target_position, ratio)
		# Additional logic to interpolate the rotation if needed
	else:
		interpolating = false
		self.global_position = target_position
