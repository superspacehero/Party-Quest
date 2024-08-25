@tool
extends Control
class_name CenteredControl

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	center()
	resized.connect(center)

# Called every time the node is resized.
func center() -> void:
	pivot_offset = size * 0.5

# This boolean is used as a button to center the control.
@export var center_control: bool = false:
	set(value):
		center()
		center_control = false