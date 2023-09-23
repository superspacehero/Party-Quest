extends Node
class_name InputManager

static var instance: InputManager = null

# Resource to instantiate when a new device is detected
@export var thing_input_prefab: PackedScene = null

# List of actions you want to explicitly process (whitelist)
var allowed_actions: Array = [
	"move_up", "move_down", "move_left", "move_right",
	"aim_up", "aim_down", "aim_left", "aim_right",
	"left_trigger", "right_trigger",
	"primary", "secondary", "tertiary", "quaternary", 
	"pause"
]

var connected_devices: Array[int]

func _ready():
	if not instance:
		instance = self

func is_device_joined(device: int) -> bool:
	# We have to do this in a different way from find, as find will return -1 if the device is not found
	var has_device = connected_devices.has(device)
	# print("Device " + str(device) + " joined: " + str(has_device))
	return has_device

func unjoined_devices():
	var valid_devices = Input.get_connected_joypads()
	if !is_device_joined(-1):
		valid_devices.append(-1) # also consider the keyboard player (device -1 for MultiplayerInput functions)
	valid_devices = valid_devices.filter(func(device): return not is_device_joined(device))
	# print("Valid devices: " + str(valid_devices))
	return valid_devices

func _process(_delta):
	for device in unjoined_devices():
		if MultiplayerInput.is_action_just_pressed(device, "primary"):
			# New device detected! Instantiate and register.
			register_thing_input(device)
		# else:
		# 	print("Device " + str(device) + " not joined yet.")

func register_thing_input(device_id: int = -1):
	if thing_input_prefab:
		var new_instance = thing_input_prefab.instantiate() 
		self.add_child(new_instance)

		# Search for ThingInput in the children
		var new_thing_input = new_instance.find_child("Input", true, false)
		if new_thing_input:
			new_thing_input.device_id = device_id
			new_thing_input.input = DeviceInput.new(device_id)

		# Register the device
		connected_devices.append(device_id)

	# print("Registered device: " + str(device_id))

func unregister_thing_input(device_id: int = -1):
	connected_devices.erase(device_id)
