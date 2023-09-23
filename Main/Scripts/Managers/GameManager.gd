extends Node
class_name GameManager

enum GameMode
{
	Other,
	Play,
	Make
}

# -------------------------
# Singleton
# -------------------------
static var instance = null

# -------------------------
# Game variables
# -------------------------

# References to nodes for the level scene and the level editor scene
@export_file("*.tscn") var level_scene
@export_file("*.tscn") var level_editor_scene

static var gameMode = GameMode.Other

# References to nodes for a touch controls "menu" and an empty "menu" to display when we don't want to show touch controls
@export_node_path("Menu") var touch_controls_menu
@export_node_path("Menu") var empty_menu

# Function to display the touch controls if we are on a mobile device
static func enable_touch_controls():
	if instance == null:
		return

	if OS.get_name() == "Android" or OS.get_name() == "iOS":
		instance.touch_controls_menu.Select()
	else:
		instance.empty_menu.Select()

# Function to hide the touch controls
static func disable_touch_controls():
	if instance == null:
		return

	instance.empty_menu.Select()

# -------------------------
# Level
# -------------------------

static func start_game(showLevelIntro):
	if instance == null:
		return

	if showLevelIntro:
		# Do the gdscript equivalent of gameObject.SetActive(true) on the levelIntroUI
		instance.levelIntroUI
	# else:


# Called when the node enters the scene tree for the first time.
func _ready():
	if instance == null:
		instance = self

	# spawnPlayers()

	# disableTouchControls()

	# if gameMode == GameMode.Play:
		# GameplayCamera.instance.centerCamera()
		
	start_game(gameMode == GameMode.Play)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
