extends ColorRect
class_name Iris

enum IrisState
{
	NONE = 0,
	CLOSING = -1,
	OPENING = 1
}

# Singleton
static var instance

# Exports
@export var starting_state: IrisState = IrisState.NONE
@export_range(0.001, 10, 0.001) var iris_time: float = 0.5
@export_range(0, 10, 0.5) var iris_wait_time: float = 0.5

# Signals
signal on_iris_open
signal on_iris_close

var mat

# Interpolation related variables
var interpolating = false
var elapsed_time = 0.0
var start_value = 1.0
var target_value = 0.0
var signal_name = ""

func _ready():
	instance = self

	mat = material as ShaderMaterial
	if not mat:
		printerr("No ShaderMaterial found!")
		return

	# Start the interpolation based on starting_state
	animate_iris()

func close_iris():
	mat.set_shader_parameter("Circle_Size", 1.0)

func open_iris():
	mat.set_shader_parameter("Circle_Size", 0.0)

func animate_iris():
	# Setup interpolation values based on starting_state
	match int(starting_state):
		IrisState.OPENING:
			start_value = 1.0
			target_value = 0.0
			signal_name = "on_iris_open"
			close_iris()
		IrisState.CLOSING:
			start_value = 0.0
			target_value = 1.0
			signal_name = "on_iris_close"
			open_iris()
		_:
			return

	# Start interpolating
	interpolating = true
	
func _process(delta):
	if interpolating:
		elapsed_time += delta
		if elapsed_time < iris_wait_time:
			return
		if elapsed_time < iris_time + iris_wait_time:
			var ratio = (elapsed_time - iris_wait_time) / iris_time
			var current_value = lerp(start_value, target_value, ratio)
			mat.set_shader_parameter("Circle_Size", current_value)
		else:
			interpolating = false
			mat.set_shader_parameter("Circle_Size", target_value)
			emit_signal(signal_name)
			elapsed_time = 0.0
			
# Static
@export var scene_to_load: PackedScene
@export var load_scene: bool:
	set(value):
		if value:
			instance.iris_to_scene(scene_to_load)

static func iris_to_scene(scene: PackedScene):
	instance.scene_to_load = scene
	instance.on_iris_close.connect(scene_load)
	instance.starting_state = IrisState.CLOSING
	instance.animate_iris()

static func scene_load():
	instance.get_tree().change_scene_to_packed(instance.scene_to_load)