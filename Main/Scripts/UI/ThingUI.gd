extends Control
class_name ThingUI

@export var thing: GameThing:
    set(value):
        thing = value

        if thing == null:
            return

        if thing_name_text != null:
            thing_name_text.text = thing.thing_name
        
        if thing_portrait_image != null:
            thing_portrait_image.texture = thing.thing_portrait

        if thing_value_text != null:
            thing_value_text.text = String.num_int64(thing.thing_value)

@export var load_thing_on_ready: bool = true

# Make this a generic text node so it can be a Label, RichTextLabel, TextEdit, etc.
@export_category("UI")
@export var thing_name_text: Control

@export var thing_portrait_image: TextureRect
@export var thing_value_text: Label

func _ready() -> void:
    if load_thing_on_ready:
        thing = thing