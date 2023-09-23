extends TextureRect
class_name Iris

enum IrisState
{
	NONE = 0,
	CLOSING = -1,
	OPENING = 1
}

# Exports
@export var startingState: IrisState = IrisState.NONE
@export_range(0.001, 10, 0.001) var irisTime: float = 0.5
@export_range(0, 10, 0.5) var irisWaitTime: float = 0.5

# Signals (equivalent to UnityEvents)
signal on_iris_open
signal on_iris_close

var mat
var tween  # Used for interpolation

func _ready():
	mat = material as ShaderMaterial
	if not mat:
		printerr("No ShaderMaterial found!")
		return

	# Initialize Tween
	tween = create_tween()

	match int(startingState):
		IrisState.NONE:
			# Do nothing for None
			pass
		IrisState.CLOSING:
			close_iris()
		IrisState.OPENING:
			open_iris()

	if irisWaitTime > 0:
		var timer = Timer.new()
		timer.wait_time = irisWaitTime
		timer.one_shot = true
		timer.connect("timeout", Callable(self, "animate_iris"))
		add_child(timer)
		timer.start()
	else:
		animate_iris()

func close_iris():
	mat.set_shader_param("_Circle_Size", 1.0)
	# You might want to handle the visibility or shader's effect here

func open_iris():
	mat.set_shader_param("_Circle_Size", 0.0)
	# You might want to handle the visibility or shader's effect here

func animate_iris():
	# tween.remove_all()

	var start_value
	var target_value
	var signal_name

	match int(startingState):
		IrisState.NONE:
			start_value = 1.0
			target_value = 0.0
			signal_name = "on_iris_open"
		IrisState.CLOSING:
			start_value = 0.0
			target_value = 1.0
			signal_name = "on_iris_close"
		_:
			return  # Do nothing for other cases

	# tween.interpolate_property(mat, "_Circle_Size", start_value, target_value, irisTime, Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	# tween.connect("completed", Callable(self, "_on_tween_complete"), signal_name)
	# tween.start()

func _on_tween_complete(signal_name):
	emit_signal(signal_name)
