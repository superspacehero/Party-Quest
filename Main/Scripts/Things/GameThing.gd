extends Node3D
class_name GameThing

# Called when the node enters the scene tree for the first time.
func _ready():
	max_health = variables.get("health", 100)
	health = max_health
	
	for path in inventory_paths:
		var node = get_node(path)
		# print("Node fetched for path: ", path, " is: ", node)
		if node:
			inventory.append(node)
	# print("Final Inventory:", inventory)

	# set_sorting_offset_to_position()

# Variables
@export var thing_root : Node3D:
	get:
		if not thing_root:
			thing_root = self
			
		return thing_root

@export var thing_visual : VisualInstance3D
@export var relative_sorting_offset: float = 0.0:
	set(value):
		relative_sorting_offset = value
		if thing_visual:
			thing_visual.sorting_offset = relative_sorting_offset
	get:
		return relative_sorting_offset

@export var thing_name : String
@export var thing_description : String = ""
@export var thing_value : int = 0

var thing_type : String
var thing_subtype : String

@export var inventory_paths : Array[NodePath] = []
var inventory : Array = []

var thing_top: Node3D = null:
	get:
		if thing_top == null:
			thing_top = find_child("Top", true, true)

			if thing_top == null:
				# print("No thing_top found for ", name, ". Setting to transform.")
				thing_top = self
			# else:
			# 	print("thing_top found for ", name, ": ", thing_top)
		return thing_top
	set(value):
		thing_top = value

var health: int = 0:
	set(value):
		if value < 0:
			value = 0
			die()
		if value > max_health:
			value = max_health
		health = value
	get:
		return health

var max_health: int = 100

@export var variables: Dictionary

# Functions
func get_thing_type() -> String:
	return "Game"
func get_thing_subtype() -> String:
	return "Game"

func set_sorting_offset_to_position(offset: float = 0.0):
	if thing_visual:
		thing_visual.sorting_offset = offset + relative_sorting_offset
		# print("Setting sorting offset for ", name, " to ", thing_visual.sorting_offset)

func die():
	print("Dying!")
	# Destroy self
	queue_free()

func move(_direction):
	pass
	
func primary(_pressed):
	pass

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
