extends UnsavedThing
class_name Dice

# Dice properties
signal enabled_event
signal disabled_event

signal on_value_changed

@export var dirs = [Vector3.UP, Vector3.RIGHT, Vector3.FORWARD, Vector3.BACK, Vector3.LEFT, Vector3.DOWN]

var side: Vector3

# Movement properties
var rolled: bool = false

@export var rb: RigidBody3D

@export_range(0.0, 1.0, 0.01) var stick_launch_magnitude: float = 0.99

@export var launch_force: Vector2 = Vector2(10.0, 5.0)

@export var roll_speed: float = 5.0

@export var roll_duration: float = 4.0

var roll_time: float = 0.0

# Initialization
var spawn_position: Vector3
var initialized: bool = false

func _ready():
    initialize_dice()

func initialize_dice():
    await roll_dice()

func roll_dice():
    rolled = false
    thing_value = 0
    roll_time = 0.0

    enabled_event.emit()

    rb.axis_lock_linear_x = true
    rb.axis_lock_linear_y = true
    rb.axis_lock_linear_z = true
    rb.axis_lock_angular_x = true
    rb.axis_lock_angular_y = true
    rb.axis_lock_angular_z = true

    # Disable the die's collision until it's rolled
    rb.collision_layer = 0

    rb.angular_velocity = Vector3.ZERO
    rb.gravity_scale = 0.0

    rb.position = spawn_position
    rb.rotation = Vector3.ZERO

    # Dice behavior until it's rolled: spin around
    while not rolled:
        rb.rotate_y(roll_speed * get_physics_process_delta_time())
        # Ping-pong the x-axis rotation between 0 and 180 degrees
        rb.rotate_x(roll_speed * get_physics_process_delta_time() * (1.0 - abs(sin(deg_to_rad(rb.rotation.x)))))

        await get_tree().physics_frame

    # Dice behavior while rolling
    while roll_time < roll_duration and rad_to_deg(rb.angular_velocity.length()) > 0.01:
        roll_time += get_physics_process_delta_time()

        await get_tree().physics_frame

    # Dice behavior when it's stopped rolling: find the side facing up
    var up_direction = Vector3.ZERO

    for direction in dirs:
        var dot = direction.dot(rb.global_transform.basis.y)
        if dot > up_direction.dot(rb.global_transform.basis.y):
            up_direction = direction

    thing_value = dirs.find(up_direction) + 1
    on_value_changed.emit(thing_value)

func launch(direction):
    if rolled and thing_value <= 0:
        return

    if (thing_value > 0):
        initialize_dice()

    rolled = true

    side = dirs[randi() % dirs.size()]

    direction.normalize()

    rb.gravity_scale = 1.0

    rb.rotate(side, 90.0)

    # Enable the die's collision
    rb.collision_layer = 1

    rb.axis_lock_linear_x = false
    rb.axis_lock_linear_y = false
    rb.axis_lock_linear_z = false
    rb.axis_lock_angular_x = false
    rb.axis_lock_angular_y = false
    rb.axis_lock_angular_z = false
    
    var launch_force_vec3 = Vector3(direction.x, direction.y, direction.x)
    rb.apply_impulse(launch_force_vec3 * Vector3(direction.x, 1.0, direction.y))
    rb.apply_torque_impulse(launch_force_vec3 * Vector3(direction.x, 1.0, -direction.y))

# Actions
func move(direction):
    if direction.length() >= stick_launch_magnitude:
        launch(direction)

func random_launch():
    var launch_direction: Vector2 = Vector2(randf() * 2.0 - 1.0, randf() * 2.0 - 1.0).normalized()
    launch(launch_direction)

func primary(pressed):
    if pressed:
        random_launch()