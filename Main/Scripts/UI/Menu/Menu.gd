extends Control
class_name Menu

static var current_menu = null:
	set(menu):
		if current_menu:
			current_menu.deselect()
		current_menu = menu
		print("Current menu: ", current_menu)
	get:
		return current_menu

@export var previous_menu: Menu
@export var next_menu: Menu

@export var disable_on_deselect: bool = true
@export var delay_on_following_selects: bool = true

@export var starting_button: Button

var selected: bool = false
# var first_press: bool = true
var input_ready: bool = true
var not_yet_selected: bool = true

signal on_select
signal on_deselect

var menu_effects: Array = [MenuEffect]
var disable_delay: float = 0.0

var buttons: Array = []

func add_menu_effect(effect: MenuEffect):
	menu_effects.append(effect)
	if effect.effect_delay + effect.effect_time > disable_delay:
		disable_delay = effect.effect_delay + effect.effect_time

# Called when the node enters the scene tree for the first time.
func _ready():
	if visible:
		select()
	else:
		deselect()

	# Recursively get all the buttons in the menu
	for child in GeneralFunctions.get_all_children(self):
		if child.get_class().find("Button") != - 1:
			buttons.append(child)

func select_starting_button():
	if starting_button:
		starting_button.grab_focus()

func will_delay() -> bool:
	return not_yet_selected or delay_on_following_selects

func set_buttons_disabled(disabled: bool):
	for button in buttons:
		if button:
			button.set_disabled(disabled)

func select():
	if selected:
		return

	if not visible:
		enable()

	if starting_button:
		select_starting_button()

	# first_press = true
	input_ready = false
	selected = true
	current_menu = self

	emit_signal("on_select")

	set_buttons_disabled(true)

	await get_tree().create_timer(disable_delay if will_delay() else 0.0).timeout

	input_ready = true
	not_yet_selected = false

	set_buttons_disabled(false)

	if InputManager.instance != null:
		InputManager.instance.allow_joining = true
		InputManager.instance.device_joined.connect(connect_to_device)
		connect_to_all_inputs()

func deselect():
	if not selected:
		return

	selected = false
	input_ready = false

	set_buttons_disabled(true)

	if InputManager.instance != null:
		InputManager.instance.allow_joining = false
		if InputManager.instance.device_joined.get_connections().size() > 0:
			InputManager.instance.device_joined.disconnect(connect_to_device)

	emit_signal("on_deselect")

	if disable_on_deselect:
		# Disable the menu after the delay
		await get_tree().create_timer(disable_delay).timeout
		disable()

func connect_to_all_inputs():
	if InputManager.instance != null:
		for input in InputManager.instance.get_inputs():
			connect_to_input(input as ThingInput)
	else:
		print("InputManager is null")

func connect_to_input(input: ThingInput):
	if input != null:
		if !input.inventory.has(self):
			input.inventory.append(self)
			# print("Inputs: " + str(input.inventory))

func connect_to_device(device: int):
	if InputManager.instance != null:
		var input = InputManager.instance.get_input(device)
		connect_to_input(input)

# func first_button_press():
# 	first_press = false

func secondary(pressed):
	# if first_press:
	# 	first_button_press()
	# 	return
	if input_ready and pressed:
		select_previous_menu()

func enable():
	process_mode = Node.PROCESS_MODE_INHERIT
	visible = true

func disable():
	process_mode = Node.PROCESS_MODE_DISABLED
	visible = false

func select_next_menu():
	if next_menu and input_ready:
		next_menu.select()

func select_previous_menu():
	if previous_menu and input_ready:
		previous_menu.select()