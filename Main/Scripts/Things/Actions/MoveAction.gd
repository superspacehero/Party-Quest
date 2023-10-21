extends ActionThing
class_name MoveAction

# Override thingSubType
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

func run_action() -> void:
	var number_of_dice_rolls: int = user.variables.get("movement", movement_range)
	var sum_of_dice_rolls: int = 0

	GameManager.enable_touch_controls()

	# Roll the dice
	for i in range(number_of_dice_rolls):
		Dice dice_instance = GameManager.instance.dice_pool.get_die_from_pool(user.thing_top)