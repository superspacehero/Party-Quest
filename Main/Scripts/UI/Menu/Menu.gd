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

@export var disable_on_deselect = true

@export var starting_button: Button

var selected: bool = false
var input_ready: bool = true

signal on_select
signal on_deselect

var menu_effects: Array = [MenuEffect]
var disable_delay: float = 0.0

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

func select_starting_button():
	if starting_button:
		starting_button.grab_focus()

func select():
	if selected:
		return

	if not visible:
		enable()

	if starting_button:
		select_starting_button()

	input_ready = false
	selected = true
	current_menu = self

	emit_signal("on_select")

	await get_tree().create_timer(disable_delay).timeout
	input_ready = true

func deselect():
	if not selected:
		return

	selected = false

	input_ready = false

	emit_signal("on_deselect")

	if disable_on_deselect:
		# Disable the menu after the delay
		await get_tree().create_timer(disable_delay).timeout
		disable()

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