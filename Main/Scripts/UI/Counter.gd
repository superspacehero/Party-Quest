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
    var time = 0.0
    var _rotate_time_half = rotate_time * 0.5

    var first_scale = Vector2.ONE
    var last_scale = Vector2(1, 0)
    # var original_rotation = 0.0
    # var mid_rotation = rotation_variation * ((randf() * 2) - 1.0)
    # var last_rotation = rotation_variation * ((randf() * 2) - 1.0)

    while time < 1.0:
        # rotation_degrees = lerp(original_rotation, mid_rotation, time)
        scale = first_scale.lerp(last_scale, time)
        time += get_process_delta_time() / _rotate_time_half
        await get_tree().process_frame

    time = 0.0
    draw_text()

    while time < 1.0:
        # rotation_degrees = lerp(last_rotation, original_rotation, time)
        scale = last_scale.lerp(first_scale, time)
        time += get_process_delta_time() / _rotate_time_half
        await get_tree().process_frame

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