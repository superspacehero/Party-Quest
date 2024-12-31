extends Node3D
class_name GameplayCamera

# Singleton
static var instance: GameplayCamera = null

# Camera Variables
@export var camera: Camera3D = null
@export var camera_adjust_time = 0.25
@export var camera_sensitivity = 1.5

var camera_rotation_amount: Vector2 = Vector2.ZERO
@export var camera_offset = Vector3(0, 0, 3.5)
@export var camera_rotation = Vector3(-11.25, 0, 0)
@export var rotation_limits = Vector2(-60, 60)
var current_rotation = Vector2.ZERO

var camera_objects: Array[GameThing] = []

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

func _process(delta):
	if camera_rotation_amount.length() > 0:
		if camera_rotation_amount.length() > 1:
			camera_rotation_amount = camera_rotation_amount.normalized()
		rotate_camera(camera_rotation_amount)

	if interpolating:
		center_camera_interpolation(delta)

static func rotate_camera(relative: Vector2):
	if instance == null:
		return
	instance.current_rotation.x = clamp(instance.current_rotation.x + relative.x * instance.camera_sensitivity, instance.rotation_limits.x, instance.rotation_limits.y)
	instance.current_rotation.y += relative.y * instance.camera_sensitivity
	instance.rotation_degrees.x = -instance.current_rotation.x
	instance.rotation_degrees.y = -instance.current_rotation.y

static func set_interpolating():
	if instance == null:
		return
	instance.elapsed_time = 0.0
	instance.start_position = instance.global_position
	instance.interpolating = instance.camera_objects.size() > 0

static func clear_camera_objects():
	if instance == null:
		return
	instance.camera_objects.clear()
	set_interpolating()

static func add_camera_object(game_thing: GameThing):
	if instance == null:
		return
	instance.camera_objects.append(game_thing)
	set_interpolating()

static func remove_camera_object(game_thing: GameThing):
	if instance == null:
		return
	instance.camera_objects.erase(game_thing)
	set_interpolating()

static func set_camera_object(game_thing: GameThing):
	clear_camera_objects()
	add_camera_object(game_thing)

func center_camera_interpolation(delta):
	if camera_objects.size() == 0:
		interpolating = false
		return

	elapsed_time += delta / camera_adjust_time
	var sum_pos = Vector3.ZERO
	for obj in camera_objects:
		sum_pos += obj.thing_root.global_position + (Vector3.UP * obj.thing_height * 0.5)
	var center = sum_pos / float(camera_objects.size())

	var adjusted_target = center + camera_offset

	if elapsed_time < 1.0:
		var ratio = elapsed_time
		self.global_position = start_position.lerp(adjusted_target, ratio)
	else:
		interpolating = false
		self.global_position = adjusted_target
