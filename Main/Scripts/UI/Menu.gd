extends Control
class_name Menu

static var currentMenu = null:
	set(menu):
		if currentMenu != null:
			currentMenu.deselectMenu(menu)
		currentMenu = menu
		currentMenu.show()
	get:
		return currentMenu
		
@export_node_path("Menu") var previousMenu
@export_node_path("Menu") var nextMenu

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
