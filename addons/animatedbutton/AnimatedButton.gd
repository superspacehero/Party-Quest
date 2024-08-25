class_name AnimatedButton
extends Button

# ---------- Exported Variables ---------- #
@export_range(0, 10, 0.001, "or_greater") var duration = 0.1

@export var animate_scale: bool = true
@export var animate_position: bool = false
@export var transition_type: Tween.TransitionType

@export_group("Scale Properties", "scale_")
@export var scale_hover: float = 1.2
@export var scale_pressed: float = 0.8

@export_group("Position Properties", "position_")
@export var position_hover: Vector2 = Vector2(0, -4)
@export var position_pressed: Vector2 = Vector2(0, -8)

# ---------- Private Variables ---------- #

var tween: Tween
var btn_start_scale: Vector2
var btn_start_pos

enum ButtonState
{
	NORMAL,
	HOVER,
	PRESSED
}

var button_state: ButtonState = ButtonState.NORMAL
var previous_button_state: ButtonState = ButtonState.NORMAL

# ---------- Built-in Functions ---------- #

func _ready() -> void:
	# Set the pivot offset at center
	pivot_offset = size / 2
	btn_start_pos = position
	btn_start_scale = scale

func _process(delta: float) -> void:
	set_button_state()

# ---------- Signals ---------- #

func set_button_state():
	if self.button_pressed:
		button_state = ButtonState.PRESSED
	elif self.is_hovered():
		button_state = ButtonState.HOVER
	else:
		button_state = ButtonState.NORMAL

	if button_state != previous_button_state:
		hover_scale_animation()
		hover_position_animation()

	previous_button_state = button_state



func hover_scale_animation():
	if animate_scale:
		match button_state:
			ButtonState.NORMAL:
				tweening(self, "scale", btn_start_scale, duration)
			ButtonState.HOVER:
				tweening(self, "scale", Vector2(btn_start_scale.x * scale_hover, btn_start_scale.y * scale_hover), duration)
			ButtonState.PRESSED:
				tweening(self, "scale", Vector2(btn_start_scale.x * scale_pressed, btn_start_scale.y * scale_pressed), duration)

func hover_position_animation():
	if animate_position:
		match button_state:
			ButtonState.NORMAL:
				tweening(self, "position", btn_start_pos, duration)
			ButtonState.HOVER:
				tweening(self, "position", btn_start_pos + Vector2(position_hover.x, position_hover.y), duration)
			ButtonState.PRESSED:
				tweening(self, "position", btn_start_pos + Vector2(position_pressed.x, position_pressed.y), duration)

# Global tween method
func tweening(object: Object, property: NodePath, final_value: Variant, duration: float):
	if tween != null:
		tween.kill()

	tween = create_tween().set_parallel(true).set_trans(transition_type)
	tween.tween_property(object, property, final_value, duration)
	await tween.finished
	tween.kill()
