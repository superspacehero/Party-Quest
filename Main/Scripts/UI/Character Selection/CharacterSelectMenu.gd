extends Menu
class_name CharacterSelectMenu

# Singleton
static var instance: CharacterSelectMenu = null

@export var character_select_parent: Node:
	get:
		if character_select_parent == null:
			character_select_parent = self
		return character_select_parent

@export var character_category: String = "Player"
@export var character_select_scene: PackedScene = null
@export var character_scene: PackedScene = null

@export_category("Animation")
@export var characters_selected_animator: AnimationPlayer
@export var characters_selected_animation: String = "Ready"

var character_selects: Array[CharacterSelect] = []

var characters = null

var all_character_selects_ready: bool:
	get:
		if character_selects.size() > 0:
			for character_select in character_selects:
				if character_select.selected_character == null:
					return false
			return true
		return false
var previous_character_selects_ready: bool = false


func check_all_character_selects_ready():
	var all_ready = all_character_selects_ready
	if characters_selected_animator != null and all_ready != previous_character_selects_ready:
		if all_ready:
			characters_selected_animator.play(characters_selected_animation)
		else:
			characters_selected_animator.play_backwards(characters_selected_animation)

	print("All character selects ready: " + str(all_ready))
	
	previous_character_selects_ready = all_ready

func load_characters():
	var character: CharacterThing = character_scene.instantiate()
	characters = GameThing.load_things("user://" + character.get_thing_type() + "s/" + character_category)
	if CharacterSelectMenu.instance != null:
		for character_select in CharacterSelectMenu.instance.character_selects:
			character_select.update_character_select()
	character.queue_free()

	print("Found " + str(characters.size()) + " characters")

func select_next_menu():
	# Save all players and their selected characters
	GameManager.players.clear()

	for character_select in character_selects:
		GameManager.players.append(GameManager.PlayerAndCharacter.new(character_select.player, character_select.selected_character))

	super.select_next_menu()

func select():
	super.select()

	instance = self

	if characters == null:
		load_characters()

func deselect():
	super.deselect()

	instance = null

	# Reset all character selects
	for character_select in character_selects:
		character_select.queue_free()

	character_selects.clear()

func primary(pressed):
	if input_ready and pressed:
		if all_character_selects_ready:
			select_next_menu()

		for input in InputManager.instance.get_inputs():
			if input.primary_action:
				for character_select in character_selects:
					if input.inventory.has(character_select):
						return

				add_character_select(input)

func add_character_select(input: ThingInput) -> CharacterSelect:
	if character_selects.size() < InputManager.instance.max_devices:
		var character_select = character_select_scene.instantiate()
		character_select_parent.add_child(character_select)
		character_select = character_select as CharacterSelect

		character_selects.append(character_select)

		if input != null:
			character_select.input = input
			input.inventory.append(character_select)

		return character_select
	return null

func connect_to_input(input: ThingInput):
	if input != null:
		if !input.inventory.has(self):
			input.inventory.append(self)
			# print("Inputs: " + str(input.inventory))

func secondary(pressed):
	if input_ready and pressed:
		if character_selects.size() == 0:
			select_previous_menu()