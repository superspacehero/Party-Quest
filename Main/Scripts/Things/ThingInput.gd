extends UnsavedThing
class_name ThingInput

var input
var device_id: int = 0

@export var is_player = true

# AI Variables
var action_coroutine = null
var targets = [] # This should be an array of GameThing nodes
var current_target_index = 0
var can_control = false
enum AIState { CHOOSING_ACTION, IDLING, MOVING, ATTACKING, HEALING, FLEEING, ENDING_TURN }
var action_delay = 1.0

var move_action: Vector2 = Vector2.ZERO:
	set(value):
		if value != move_action:
			for game_thing in inventory:
				if game_thing.has_method("move"):
					game_thing.move(value)
					# print(game_thing.name + " movement: " + str(value))
			move_action = value
	get:
		return move_action

var primary_action: bool = false:
	set(value):
		if value != primary_action:
			primary_action = value
			for game_thing in inventory:
				if game_thing.has_method("primary"):
					game_thing.primary(value)

var secondary_action: bool = false:
	set(value):
		if value != secondary_action:
			secondary_action = value
			for game_thing in inventory:
				if game_thing.has_method("secondary"):
					game_thing.secondary(value)

var tertiary_action: bool = false:
	set(value):
		if value != tertiary_action:
			tertiary_action = value
			for game_thing in inventory:
				if game_thing.has_method("tertiary"):
					game_thing.tertiary(value)

var quaternary_action: bool = false:
	set(value):
		if value != quaternary_action:
			quaternary_action = value
			for game_thing in inventory:
				if game_thing.has_method("quaternary"):
					game_thing.quaternary(value)

var left_trigger_action: bool = false:
	set(value):
		if value != left_trigger_action:
			left_trigger_action = value
			for game_thing in inventory:
				if game_thing.has_method("left_trigger"):
					game_thing.left_trigger(value)

var right_trigger_action: bool = false:
	set(value):
		if value != right_trigger_action:
			right_trigger_action = value
			for game_thing in inventory:
				if game_thing.has_method("right_trigger"):
					game_thing.right_trigger(value)

var pause_action: bool = false:
	set(value):
		if value != pause_action:
			pause_action = value
			for game_thing in inventory:
				if game_thing.has_method("pause"):
					game_thing.pause(value)

func _input(_event):
	if input and is_player:
		move_action = input.get_vector("move_left", "move_right", "move_up", "move_down")
		
		left_trigger_action = input.is_action_pressed("left_trigger")
		right_trigger_action = input.is_action_pressed("right_trigger")
		primary_action = input.is_action_pressed("primary")
		secondary_action = input.is_action_pressed("secondary")
		tertiary_action = input.is_action_pressed("tertiary")
		quaternary_action = input.is_action_pressed("quaternary")
		pause_action = input.is_action_pressed("pause")

# Called when the node is destroyed
func tree_exiting():
	InputManager.instance.unregister_thing_input(device_id)
