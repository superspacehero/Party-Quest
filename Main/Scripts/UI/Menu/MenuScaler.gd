extends MenuEffect
class_name MenuScaler

@export var scale_selected_deslected: Vector2 = Vector2(1.0, 0.0)

var menu_scale: float = 1.0:
	set(value):
		menu.scale = Vector2(value, value)

		menu_scale = value

func selected_effect():
	menu_scale = 0.0
	center_pivot()

	super.selected_effect()
	delay_effect()
	tween.tween_property(self, "menu_scale", scale_selected_deslected.x, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	menu_scale = 1.0
	center_pivot()

	super.deselected_effect()
	delay_effect()
	tween.tween_property(self, "menu_scale", scale_selected_deslected.y, effect_time).set_trans(effect_transition_type)

func center_pivot():
	menu.pivot_offset = Vector2(menu.size.x * 0.5, menu.size.y * 0.5)