extends MenuEffect
class_name MenuScaler

var menu_scale: float = 1.0:
	set(value):
		scale = Vector2(value, value)

		menu_scale = value

func selected_effect():
	menu_scale = 0.0
	center_pivot()

	super.selected_effect()
	tween.tween_interval(effect_delay)
	tween.tween_property(self, "menu_scale", 1.0, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	menu_scale = 1.0
	center_pivot()

	super.deselected_effect()
	tween.tween_interval(effect_delay)
	tween.tween_property(self, "menu_scale", 0.0, effect_time).set_trans(effect_transition_type)

func center_pivot():
	pivot_offset = Vector2(size.x * 0.5, size.y * 0.5)