extends MenuEffect
class_name MenuAnchorMover

@export var selected_anchors: Vector4
@export var deselected_anchors: Vector4

var menu_anchors: float = 1.0:
	set(value):
		menu_anchors = value
		anchors = deselected_anchors.lerp(selected_anchors, menu_anchors)

		anchor_left = anchors.x
		anchor_top = anchors.y
		anchor_right = anchors.z
		anchor_bottom = anchors.w

var anchors = Vector4(0.0, 0.0, 1.0, 1.0)

func selected_effect():
	menu_anchors = 0.0
	center_pivot()

	super.selected_effect()
	delay_effect()
	tween.tween_property(self, "menu_anchors", 1.0, effect_time).set_trans(effect_transition_type)

func deselected_effect():
	menu_anchors = 1.0
	center_pivot()

	super.deselected_effect()
	delay_effect()
	tween.tween_property(self, "menu_anchors", 0.0, effect_time).set_trans(effect_transition_type)

func center_pivot():
	pivot_offset = Vector2(size.x * 0.5, size.y * 0.5)