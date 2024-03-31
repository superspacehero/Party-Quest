extends MenuEffect
class_name MenuFader

func selected_effect():
	super.selected_effect()
	tween.tween_interval(effect_delay)
	tween.tween_property(self, "modulate:a", 1, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	super.deselected_effect()
	tween.tween_interval(effect_delay)
	tween.tween_property(self, "modulate:a", 0, effect_time).set_trans(effect_transition_type)