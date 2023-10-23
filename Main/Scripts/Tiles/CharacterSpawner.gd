extends GameThing
class_name CharacterSpawner

func get_thing_type() -> String:
    return "CharacterSpawner"

@export var character_string: String
@export var spawn_at_start: bool = true

func spawn_character():
    if character_string == "":
        return

    # Deserialize the character string into a scene
    # TO clarify, this is NOT a path. This is a string that, when deserialized, will become a scene
    var character_scene: PackedScene = JSON.parse_string(character_string)
    
    # Create the character
    character_scene.instance()