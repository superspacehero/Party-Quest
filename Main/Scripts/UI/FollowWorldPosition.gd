extends Control
class_name FollowWorldPosition

static var instance

@export var target: GameThing
@export_range(0.0, 1.0) var follow_height: float = 0.5

func _ready():
    move()

func _process(_delta):
    move()

func move():
    if target:
        position = GameplayCamera.instance.camera.unproject_position(target.position.lerp(target.thing_top.position, follow_height))