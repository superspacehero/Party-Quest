extends Control
class_name Menu

static var current_menu = null:
	set(menu):
		if current_menu:
			current_menu.deselect()
		current_menu = menu

		if current_menu:
			current_menu.visible = true
	get:
		return current_menu
		
@export var previous_menu : Menu
@export var next_menu : Menu

@export var disable_on_deselect = true

@export var starting_button : Button

var selected = false


# Called when the node enters the scene tree for the first time.
func _ready():
	if !hidden:
		select()

func select_starting_button():
	if starting_button:
		starting_button.grab_focus()

func select():
	if selected:
		return

	if not is_visible_in_tree():
		visible = true
		set_process_input(true)

	if starting_button:
		select_starting_button()

	selected = true
	current_menu = self

func deselect():
	if not selected:
		return

	if disable_on_deselect:
		set_process_input(false)
		visible = false

	selected = false
	current_menu = null

func select_next_menu():
	if next_menu:
		next_menu.select()

func select_previous_menu():
	if previous_menu:
		previous_menu.select()