extends Node
class_name GameManager

enum GameMode
{
	OTHER,
	PLAY,
	MAKE
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

static var game_mode = GameMode.OTHER

# References to nodes for a touch controls "menu" and an empty "menu" to display when we don't want to show touch controls
@export var touch_controls_menu : Menu
@export var empty_menu : Menu

# Function to display the touch controls if we are on a mobile device
static func enable_touch_controls():
	if instance == null:
		return

	if OS.get_name() == "Android" or OS.get_name() == "iOS":
		instance.touch_controls_menu.select()
	else:
		instance.empty_menu.select()

# Function to hide the touch controls
static func disable_touch_controls():
	if instance == null:
		return

	instance.empty_menu.select()

# -------------------------
# Level
# -------------------------

# Save the state of the scene
static func save_state():
	if instance == null:
		return

	# Export this scene to a PackedScene
	var packed_scene = PackedScene.new()
	packed_scene.pack(instance)

	# Save the PackedScene to a file
	ResourceSaver.save(packed_scene, "user://save_game.res")

static func load_state():
	if instance:
		instance.queue_free()

	# Load the PackedScene from the file
	var packed_scene = ResourceLoader.load("user://save_game.res")
	instance = packed_scene.instance()

	# Add the instance to the scene tree
	instance.get_tree().get_root().add_child(instance)

# -------------------------
# Things
# -------------------------

@export_category("Thing Pools")
@export var dice_pool: ObjectPool
@export var damage_indicator_pool: ObjectPool
@export var heal_indicator_pool: ObjectPool
@export var footstep_pool: ObjectPool

func damage_effect(damage_amount: int, thing: GameThing, thing_height: float, damage_position: Vector3):
	if damage_amount > 0:
		var damage_indicator: NumberVisual = damage_indicator_pool.get_object_from_pool(damage_position)
		damage_indicator.follow_world_position.target = thing
		damage_indicator.follow_world_position.follow_height = thing_height
		damage_indicator.number = damage_amount
		damage_indicator.show()

# -------------------------
# Functions
# -------------------------

# Function to start the game
static func start_game(show_level_intro: bool):
	if instance == null:
		return

	if show_level_intro:
		instance.level_intro_ui.show()
	else:
		# Wait the delay before starting the game
		await instance.change_character_delay
		set_next_character(false)
		control_next_character()

# Function to receive notifications
func _notification(what):
	if what == NOTIFICATION_WM_WINDOW_FOCUS_IN:
		# If the game is paused, unpause it
		if get_tree().paused:
			get_tree().paused = false

	if what == NOTIFICATION_WM_WINDOW_FOCUS_OUT:
		# If the game is not paused, pause it
		if !get_tree().paused:
			get_tree().paused = true

	if what == NOTIFICATION_WM_CLOSE_REQUEST:

		# Save the state of the game
		instance.save_state()

		# Quit the game
		get_tree().quit()

# -------------------------
# Players
# -------------------------

# The inputs
var inputs: Array = []

@export_category("Characters")
# The player scene
@export var character_scene: PackedScene

# Spawn the players
func spawn_players():
	var player_spawners = get_tree().get_nodes_in_group("PlayerSpawner")
	var player_spawners_list = []

	for player_spawner in player_spawners:
		player_spawners_list.append(player_spawner)

	for input in inputs:
		if player_spawners.size() > 0:
			if player_spawners_list.size() > 0:
				# Get a random player spawner
				var player_spawner = player_spawners_list[randi() % player_spawners_list.size()]

				# Spawn the player
				spawn_player(input, player_spawner)

				# Remove the player spawner from the list
				player_spawners_list.erase(player_spawner)
		else:
			# If there are no player spawners, spawn the player at the world position of cell (0, 0) in the tilemap
			spawn_player(input, Vector3.ZERO)

# Spawn a player
func spawn_player(input: ThingInput, position = null):
	if character_scene == null:
		return

	var player = character_scene.instance() as CharacterThing

	if position is Vector3:
		player.position = position

	if position is PlayerSpawner:
		player.position = (position as PlayerSpawner).position

	player.input = input

	input.inventory.append(player)

@export var cpu_player_prefab: PackedScene
var cpu_player_input: ThingInput:
	get:
		if cpu_player_input == null:
			cpu_player_input = cpu_player_prefab.instance() as ThingInput

		return cpu_player_input

func attach_cpu_player(character: CharacterThing):
	if character == null:
		return

	cpu_player_input.inventory.append(character)
	character.input = cpu_player_input


# -------------------------
# Characters
# -------------------------

# The current character index
var current_character_index = 0

# The current character
static var current_character: CharacterThing = null:
	get:
		if instance == null or not instance.characters or instance.characters.count <= 0:
			return null

		return instance.characters_in_current_team[instance.current_character_index]

# The characters
var characters: Array = []
# The teams
var teams: Array = []
# The characters in the current team
var characters_in_current_team: Array = []
# The character starting actions
@export var character_starting_actions: Array = []

# The delay when changing characters
var change_character_delay = 1.0

# The current team index
var current_team_index:
	set(value):
		if value >= teams.size():
			value = 0
		
		characters_in_current_team.clear()
		for character in characters:
			if !teams.has(character.character_team):
				teams.append(character.character_team)

static func next_character():
	set_players_controllable(false)
	set_next_character(true)

static func set_players_controllable(controllable: bool):
	if instance == null:
		return

	for input in instance.inputs:
		input.controllable = controllable

static func set_next_character(show_next_character_ui: bool = true):
	if instance == null or not instance.characters or instance.characters.count == 0:
		return

	instance.current_character_index += 1
	if instance.current_character_index >= instance.characters_in_current_team.count:
		instance.current_team_index += 1

	GameplayCamera.instance.set_camera_object(current_character)
	
	if show_next_character_ui:
		instance.next_character_ui.show()

static func control_next_character():
	if instance == null:
		return
		
	if instance.character_starting_actions and instance.character_starting_actions.count > 0:
		for input in instance.inputs:
			for game_thing in input.inventory:
				if game_thing is CharacterThing:
					var character = game_thing as CharacterThing

					# Remove the existing override starting action
					if character.override_starting_action:
						character.override_starting_action.queue_free()

					# Add the new override starting action
					if instance.character_starting_actions.count > instance.inputs.index_of(input):
						character.override_starting_action = instance.character_starting_actions[instance.inputs.index_of(input)].instance()
					else:
						character.override_starting_action = instance.character_starting_actions[instance.character_starting_actions.count - 1].instance()

	for input in instance.inputs:
		input.can_control = (current_character.input == input)

	if current_character:
		current_character.my_turn()

# Called when the node enters the scene tree for the first time.
func _ready():
	if instance == null:
		instance = self

	spawn_players()

	instance.disable_touch_controls()

	if game_mode == GameMode.PLAY:
		GameplayCamera.instance.center_camera()
		
	GameManager.start_game(game_mode == GameMode.PLAY)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
