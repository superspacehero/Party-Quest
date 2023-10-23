extends ActionThing
class_name MoveAction

# Override thing_subtype
func get_thing_subtype() -> String:
	return "Move"

# The number of spaces the character can move
var movement_range: int = 3

# The height the character can jump
var jump_height: float = 1.0

# The direction the character is moving in
var movement: Vector3 = Vector3(0, 0, 0)

# The set of valid grid spaces within the movement range
var valid_spaces: Array = []

# Whether or not the dice roll has finished
var dice_roll_finished: bool = false

# The current dice value
var current_dice_value: int = 0

# The callback for when the dice roll is finished
func _on_dice_roll_finished(dice_value: int) -> void:
	current_dice_value = dice_value
	dice_roll_finished = true

func run_action() -> void:
	var number_of_dice_rolls: int = user.variables.get("movement", movement_range)
	var sum_of_dice_rolls: int = 0

	GameManager.enable_touch_controls()

	# Roll the dice
	for i in range(number_of_dice_rolls):
		dice_roll_finished = false
		var dice_instance: Dice = GameManager.instance.dice_pool.get_object_from_pool(user.thing_top.position)
		user.attach_thing(dice_instance)
		dice_instance.on_value_changed.connect(_on_dice_roll_finished, CONNECT_ONE_SHOT)

		# Wait until the dice roll is finished
		await dice_roll_finished

		# Add the dice value to the sum
		sum_of_dice_rolls += current_dice_value

		# Update the counter
		if Counter.instance:
			Counter.instance.count = sum_of_dice_rolls

		user.detach_thing(dice_instance)
		GameManager.instance.dice_pool.return_object_to_pool(dice_instance)

	# Set the movement range based on the sum of dice rolls
	movement_range = sum_of_dice_rolls

	# Enable movement control
	if user is CharacterThing:
		var character: CharacterThing = user as CharacterThing
		character.input.can_control = true
		character.can_move = true
		character.can_jump = true
		
		jump_height = character.jump_height

	if AStarPathfinding.instance:
		# Calculate the set of valid grid spaces within the number of spaces the character can move
		valid_spaces = AStarPathfinding.instance.get_points_in_radius(user.position, movement_range, Vector2(jump_height, -1.0))

		AStarPathfinding.instance.display_nodes(valid_spaces)
		AStarPathfinding.instance.color_node_objects(valid_spaces)
		AStarPathfinding.instance.color_node_object(current_point, AStarPathfinding.instance.current_color)

	# The previous position of the user
	var previous_position: Vector3 = user.position
	var previous_point = current_point

	# While the user is still moving and hasn't stopped their movement turn
	while (action_running):
		# if the user has moved
		if (user.position != previous_position):
			# Update position to the grid-based position of the user
			thing_position = user.position

			# If the current position is not a valid space
			if (not valid_spaces.has(current_point)):
				movement = user.position - previous_position

				# Check the point in the direction of the x-axis
				var x_point = AStarPathfinding.instance.astar.get_closest_point(previous_position + Vector3(movement.x, 0, 0))

				# Check the point in the direction of the z-axis
				var z_point = AStarPathfinding.instance.astar.get_closest_point(previous_position + Vector3(0, 0, movement.z))

				# Revert the appropriate axes to the previous position
				if (not valid_spaces.has(x_point)):
					user.position.x = previous_position.x
				if (not valid_spaces.has(z_point)):
					user.position.z = previous_position.z

				current_point = previous_point
			elif AStarPathfinding.instance.check_point_occupied(current_point):
				current_point = previous_point

			if previous_point != current_point:
				AStarPathfinding.instance.color_node_object(previous_point)
				AStarPathfinding.instance.color_node_object(current_point, AStarPathfinding.instance.current_color)

		# Update the previous position
		previous_position = current_point
		previous_position = user.position

		await get_tree().process_frame

	# Hide the valid grid spaces
	AStarPathfinding.instance.hide_nodes()

	# Disable movement control
	if user is CharacterThing:
		var character: CharacterThing = user as CharacterThing
		character.can_control = CharacterThing.control_level.NONE

	user.position = thing_position

	# Occupy the point
	user.occupy_point(current_point)

	# Hide the counter
	if Counter.instance:
		Counter.instance.count = 0

	finalize_action()

func secondary(pressed):
	if pressed:
		if user is CharacterThing:
			var character: CharacterThing = user as CharacterThing
			character.can_control = CharacterThing.control_level.FULL if character.can_control == CharacterThing.control_level.NONE else CharacterThing.control_level.NONE

			if character.can_control != CharacterThing.control_level.FULL:
				character.display_actions(true)
			else:
				GameManager.enable_touch_controls()