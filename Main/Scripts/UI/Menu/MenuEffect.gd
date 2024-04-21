extends Control
class_name MenuEffect

@export var menu: Menu
@export var effect_delay: float = 0
@export var effect_time: float = 0.2
@export var effect_transition_type: Tween.TransitionType = Tween.TransitionType.TRANS_QUAD

var tween: Tween
var is_deselected_effect_running: bool = false

func _ready():
	if menu:
		menu.add_menu_effect(self)
		menu.on_select.connect(selected_effect)
		menu.on_deselect.connect(deselected_effect)

func prepare_tween():
	if tween == null or tween.finished:
		tween = create_tween()

func selected_effect():
	prepare_tween()

func deselected_effect():
	prepare_tween()

func delay_effect():
	if menu.will_delay():
		tween.tween_interval(effect_delay)