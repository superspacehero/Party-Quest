extends MenuEffect
class_name MenuTextDisplayer

func selected_effect():
	super.selected_effect()
	self.visible_ratio = 0
	
	self.visible_characters_behavior = TextServer.VisibleCharactersBehavior.VC_GLYPHS_LTR

	tween.tween_interval(effect_delay)
	tween.tween_property(self, "visible_ratio", 1, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	super.deselected_effect()
	self.visible_ratio = 1

	self.visible_characters_behavior = TextServer.VisibleCharactersBehavior.VC_GLYPHS_RTL

	tween.tween_property(self, "visible_ratio", 0, effect_time).set_trans(effect_transition_type)
