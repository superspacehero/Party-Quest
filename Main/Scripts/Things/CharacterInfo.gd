extends Resource
class_name CharacterInfo

# The name of the character.
@export var name: String

# The description of the character.
@export var description: String

# The portrait of the character.
@export var portrait: Texture

# The value of the character.
@export var value: int

# An array to store the paths to the PackedScene of each character part.
@export var character_parts: Array[PackedScene]

# A dictionary to store the variables.
@export var variables: Dictionary

static func load_characters(character_category: String = "Player") -> Array:
	check_character_directory()

	var characters: Array = []

	var character_files: PackedStringArray = DirAccess.get_files_at("user://Characters/" + character_category)
	for character_file in character_files:
		var character_info: CharacterInfo = load("user://Characters/" + character_category + "/" + character_file)
		characters.append(character_info)

	return characters

static func check_character_directory() -> void:
	if not DirAccess.dir_exists_absolute("user://Characters"):
		DirAccess.make_dir_absolute("user://Characters")
