extends MenuEffect
class_name MenuFader

@export var start_from_initial = true
@export_range(0, 1) var initial_alpha = 0
@export_range(0, 1) var selected_alpha = 1
@export_range(0, 1) var deselected_alpha = 0

func selected_effect():
	super.selected_effect()
	if start_from_initial:
		self.modulate.a = initial_alpha
		start_from_initial = false
	delay_effect()
	tween.tween_property(self, "modulate:a", selected_alpha, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	super.deselected_effect()
	if start_from_initial:
		self.modulate.a = initial_alpha
		start_from_initial = false
	delay_effect()
	tween.tween_property(self, "modulate:a", deselected_alpha, effect_time).set_trans(effect_transition_type)