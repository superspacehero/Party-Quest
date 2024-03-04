extends Control
class_name CharacterUI

var character_category: String = "Player"

var character_name: String:
    get:
        return character_info.name
    set(value):
        character_info = character_info.load_character(value, character_category)

@export var character_info: CharacterInfo:
    set(value):
        character_info = value

        if character_name_text != null:
            character_name_text.text = character_info.name
        
        if character_portrait_image != null:
            character_portrait_image.texture = character_info.portrait

        if character_value_text != null:
            character_value_text.text = String.num_int64(character_info.value)

@export var load_character_on_ready: bool = true

@export var character_name_text: Label
@export var character_portrait_image: TextureRect
@export var character_value_text: Label

func _ready() -> void:
    if load_character_on_ready:
        character_name = character_name