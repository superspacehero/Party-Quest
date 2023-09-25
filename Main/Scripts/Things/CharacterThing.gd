extends UnsavedThing
class_name CharacterThing

# 1. Member Variables/Properties

@export var character_body : CharacterBody3D = null
@export var gameplay_camera : GameplayCamera = null
@export var character_base : ThingSlot = null
@export var character_collider : CollisionShape3D = null

@export_category("Movement")
@export var character_speed : float = 4  # The speed at which the character moves.
@export var jump_height: float = 1  # The height of the character's jump.
@export var jump_offset: float = 0.15  # The offset of the character's jump.
@export var gravity: float = 50  # The gravity of the character.

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
	gameplay_camera.set_camera_object(self, 0.5, true)
	character_body.velocity = Vector3.ZERO
	
	rotate_base(Vector3.FORWARD if rotation_behavior != movement_rotation_behavior.LEFT_RIGHT_ROTATION else Vector3.RIGHT)

func _physics_process(delta):
	var movement_vector = calculate_movement_direction() * character_speed
	velocity.x = movement_vector.x
	velocity.z = movement_vector.z

	# if movement.z != 0:
	# 	set_sorting_offset_to_position()
	# 	for part in parts:
	# 		var found_part = find_part_in_children(part)
	# 		found_part.set_sorting_offset_to_position()

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
var added_parts: Array = [Node3D]

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

# Helper function to find the CharacterPartThing instance in children nodes recursively
func find_part_in_children(node: Node) -> CharacterPartThing:
	if not node:
		return null
	for child in node.get_children():
		if child is CharacterPartThing:
			return child as CharacterPartThing
		else:
			var result = find_part_in_children(child)
			if result:
				return result
	return null

func attach_part(part: CharacterPartThing, parent: ThingSlot):
	if !added_parts.has(part.thing_root):
		parent.add_thing(part)
		added_parts.append(part.thing_root)

		part.thing_root.position = Vector3.ZERO
		# part_base.rotation = Vector3.ZERO
		part.thing_root.scale = Vector3.ONE

		variables.merge(part.variables)
		
		if part is HeadThing:
			thing_top = part.thing_top
		elif part is BodyThing:
			var body_thing: BodyThing = part as BodyThing
			match character_collider.shape:
				CapsuleShape3D:
					var capsule = character_collider.shape as CapsuleShape3D
					if capsule:
						capsule.radius = body_thing.collider_dimensions.x * 0.5
						capsule.height = body_thing.collider_dimensions.y
				BoxShape3D:
					var box = character_collider.shape as BoxShape3D
					if box:
						box.extents = body_thing.collider_dimensions * 0.5

		attach_parts_to_part(part)

		# print("Attached part: " + part.name + " to " + parent.name)

func attach_parts_to_part(part: CharacterPartThing):
	for slot in part.inventory:
		if slot is ThingSlot:
			attach_part_to_slot(slot, part)

func attach_part_to_slot(slot: ThingSlot, slot_part: GameThing = null):
	var attached_part_success: bool = false

	# The primary search should be within the children nodes.
	for part in parts:
		var found_part = find_part_in_children(part)
		if found_part and not slot.thing and (found_part.thing_type == slot.thing_type or found_part.thing_subtype == slot.thing_type) and !added_parts.has(found_part):
			attach_part(found_part, slot)
			attached_part_success = true

			# Set the thing's visual's sorting offset, if a VisualInstance3D has been set
			if slot_part:
				found_part.relative_sorting_offset = slot_part.relative_sorting_offset + slot.sorting_offset
			# print("Attached part: " + found_part.name + " to " + slot.name)
			# break

	if !attached_part_success:
		print("No part to attach to slot: " + slot.name)

