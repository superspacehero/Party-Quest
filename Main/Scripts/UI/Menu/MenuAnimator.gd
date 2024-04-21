extends MenuEffect
class_name MenuAnimator

@export var animator : AnimationPlayer
@export var selected_animation : String
@export var deselected_animation : String
@export var repeat_animation : bool = false

var selected_animation_played : bool = false
var deselected_animation_played : bool = false

func selected_effect():
	if selected_animation == "" or (selected_animation_played and not repeat_animation):
		return
	await get_tree().create_timer(effect_delay).timeout
	animator.stop()
	animator.play(selected_animation)
	selected_animation_played = true

func deselected_effect():
	if deselected_animation == "" or (deselected_animation_played and not repeat_animation):
		return
	animator.stop()
	animator.play(deselected_animation)
	deselected_animation_played = true