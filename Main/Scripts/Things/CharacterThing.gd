extends UnsavedThing
class_name CharacterThing

# 1. Member Variables/Properties

@export var character_body : CharacterBody3D = null
@export var gameplay_camera : GameplayCamera
@export var character_base : ThingSlot = null

@export_category("Movement")
@export var character_speed : float = 8  # The speed at which the character moves.
@export var jump_height: float = 1  # The height of the character's jump.
@export var jump_offset: float = 0.15  # The offset of the character's jump.
@export var gravity: float = 100  # The gravity of the character.

@onready var jump_full_height: float = jump_height + jump_offset
@onready var jump_velocity: float = sqrt(2 * gravity * jump_full_height)

enum control_level { NONE, MOVEMENT_ONLY, FULL }
@export var can_control: control_level = control_level.FULL
@export var can_move: bool = true
@export var can_jump: bool = true
# @onready var gravity: float = ProjectSettings.get_setting("physics/3d/default_gravity")

enum movement_rotation_behavior { NONE, FULL_ROTATION, LEFT_RIGHT_ROTATION, TOWARDS_CAMERA }
@export var rotation_behavior = movement_rotation_behavior.LEFT_RIGHT_ROTATION

var jump_input: bool = false
var is_jumping: bool = false

var velocity : Vector3 = Vector3.ZERO
var goto_rotation: float
var rotation_time: float = 0.25

var rotation_direction: Vector3 = Vector3.FORWARD
var movement: Vector3 = Vector3.ZERO

# 2. Built-in Functions

func _init():
	thing_type = "Character"

func _ready():
	super()

	assemble_character()
	gameplay_camera.set_camera_object(self, 1, true)
	character_body.velocity = Vector3.ZERO
	
	rotate_base(Vector3.FORWARD if rotation_behavior != movement_rotation_behavior.LEFT_RIGHT_ROTATION else Vector3.RIGHT)

func _physics_process(delta):
	var movement_vector = calculate_movement_direction() * character_speed
	velocity.x = movement_vector.x
	velocity.z = movement_vector.z

	if character_body.is_on_floor():
		if !jump_input:
			is_jumping = false

		if can_control != control_level.NONE and can_jump and jump_input and !is_jumping:
			velocity.y = jump_velocity
			is_jumping = true
		elif velocity.y < 0:
			velocity.y = 0
	else:
		velocity.y -= gravity * delta
	
	character_body.move_and_slide()

	character_body.velocity = velocity

func _process(delta):
	rotate_towards_goto_rotation(delta)

# 3. Movement Functions

func calculate_movement_direction() -> Vector3:
	var direction = Vector3.ZERO

	direction += Plane(gameplay_camera.basis.x,character_body.basis.y.z).normalized().normal * movement.x
	direction += Plane(gameplay_camera.basis.z,character_body.basis.y.z).normalized().normal * movement.z
	direction.y = 0

	direction = direction.normalized()
	
	if direction.length() > 0.01:
		rotate_base(direction)
	
	return direction

func rotate_base(direction: Vector3):

	match rotation_behavior:
		movement_rotation_behavior.NONE:
			pass
		movement_rotation_behavior.FULL_ROTATION:
			# Do nothing. The character will rotate in the direction of movement.
			pass
		movement_rotation_behavior.LEFT_RIGHT_ROTATION:
			if direction.x != 0:
				direction.x = sign(direction.x)

			direction = round(direction)
			
			if direction.x == 0:
				direction.x = rotation_direction.x
			
			direction.z *= 0.5
		movement_rotation_behavior.TOWARDS_CAMERA:
			direction = -gameplay_camera.basis.z

	rotation_direction = direction
	goto_rotation = atan2(rotation_direction.x, rotation_direction.z)
	# print("goto_rotation: " + str(rad_to_deg(goto_rotation)))
	
func rotate_towards_goto_rotation(delta):
	character_base.rotation.y = lerp_angle(character_base.rotation.y, Vector2(-rotation_direction.z, -rotation_direction.x).angle(), delta / rotation_time)

# 4. Event Handling Functions

func move(direction):
	# Normalize the direction to ensure constant speed.
	if direction.length() > 1:
		direction = direction.normalized()
	
	movement.x = direction.x
	movement.z = direction.y
	
	# print("move: " + str(direction))

func aim(direction):
	gameplay_camera.camera_rotation_amount = direction
	
func primary(pressed):
	jump_input = pressed

func secondary(_pressed):
	pass

func tertiary(_pressed):
	pass

func quaternary(_pressed):
	pass

func left_trigger(_pressed):
	pass

func right_trigger(_pressed):
	pass

func pause(_pressed):
	pass

# 5. Character Assembly Variables

@export_category("Character Assembly")

@export_file("*.tres") var character_info_path
var parts: Array = [CharacterPartThing]
var added_parts: Array = [CharacterPartThing]

# 6. Character Assembly Functions

func assemble_character(path: String = ""):
	clear_previous_parts()
	if path == "":
		path = character_info_path
	var character_info_resource = load(path)
	
	if character_info_resource:
		thing_name = character_info_resource.name
		thing_description = character_info_resource.description
		thing_value = character_info_resource.value

		# print("Assembling character: " + thing_name)

		for part in character_info_resource.character_parts:
			var part_instance = part.instantiate()
			if part_instance is HeadThing:
				thing_top = part_instance.thing_top
			parts.append(part_instance)
			# Set any properties on the part, such as color.

		# Attach parts to other parts.
		attach_part_to_slot(character_base)

	thing_top = thing_top

func clear_previous_parts() -> void:
	# Clear children from the base.
	for child in character_base.get_children():
		child.queue_free()

	parts.clear()
	added_parts.clear()

func attach_part(part: CharacterPartThing, parent: ThingSlot):
	if !added_parts.has(part):
		parent.add_thing(part)
		added_parts.append(part)

		part.position = Vector3.ZERO
		# part.rotation = Vector3.ZERO
		part.scale = Vector3.ONE

		variables.merge(part.variables)

		attach_parts_to_part(part)

		# print("Attached part: " + part.name + " to " + parent.name)

func attach_parts_to_part(part: CharacterPartThing):
	for slot in part.inventory:
		if slot is ThingSlot:
			attach_part_to_slot(slot)

func attach_part_to_slot(slot: ThingSlot):
	var attached_part_success: bool = false
	for part in parts:
		if part and slot and (part.thing_type == slot.thing_type or part.thing_subtype == slot.thing_type) and !added_parts.has(part):
			attach_part(part, slot)
			attached_part_success = true
			break
	if !attached_part_success:
		print("No part to attach to slot: " + slot.name)
