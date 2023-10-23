extends Node2D
class_name NumberVisual

static var instance

@export var text: RichTextLabel
@export var follow_world_position: FollowWorldPosition

var number: int = 0:
    set(value):
        number = value
        text.text = str(number)

