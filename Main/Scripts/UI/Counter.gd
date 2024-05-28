extends RichTextLabel
class_name Counter

static var instance

@export var text_to_append: String = ""
@export var rotate_time: float = 0.25
@export var rotation_variation: float = 10.0

var count: int = 0:
    set(value):
        count = value
        await flip_number()

func flip_number():
    var _rotate_time_half = rotate_time * 0.5

    var first_scale = Vector2.ONE
    var last_scale = Vector2(1, 0)
    # var original_rotation = 0.0
    # var mid_rotation = rotation_variation * ((randf() * 2) - 1.0)
    # var last_rotation = rotation_variation * ((randf() * 2) - 1.0)

    var tween = Tween.new()
    tween.interpolate_property(self, "scale", first_scale, last_scale, rotate_time, Tween.TRANS_QUAD)
    
    draw_text()

    tween.interpolate_property(self, "scale", last_scale, first_scale, rotate_time, Tween.TRANS_QUAD)

func draw_text():
    if count > 0:
        self.text = text_to_append.replace("@", str(count))
    else:
        text = ""

func _ready():
    instance = self
    draw_text()

# var timed_count = 0.0

# func _process(delta):
#     timed_count += delta
#     if timed_count >= 1.0:
#         timed_count = 0.0
#         count += 1