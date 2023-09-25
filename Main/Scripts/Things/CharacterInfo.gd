extends Resource
class_name CharacterInfo

# The name of the character.
@export var name: String

# The description of the character.
@export var description: String

# The value of the character.
@export var value: int

# An array to store the paths to the PackedScene of each character part.
@export var character_parts: Array[PackedScene]

# A dictionary to store the variables.
@export var variables: Dictionary

# Methods to serialize and deserialize the data to/from JSON.
func to_json() -> String:
	return to_json()

func from_json(json_string: String):
	from_json(json_string)
