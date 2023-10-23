extends Control
class_name Menu

static var current_menu = null:
	set(menu):
		if current_menu != null:
			current_menu.deselect_menu(menu)
		current_menu = menu
		current_menu.show()
	get:
		return current_menu
		
@export var previous_menu : Menu
@export var next_menu : Menu

@export var disable_on_deselect = true

var selected = false


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func select():
	if selected:
		return

	if not is_visible_in_tree():
		show()
		set_process_input(true)

	selected = true
	current_menu = self

func deselect():
	if not selected:
		return

	if disable_on_deselect:
		set_process_input(false)
		hide()

	selected = false
	current_menu = null