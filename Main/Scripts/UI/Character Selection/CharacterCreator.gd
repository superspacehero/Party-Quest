extends Control
class_name CharacterCreator

@export var character_part_list: CharacterPartList
@export var character_text: Control
@export var character_select: CharacterSelect

var character: CharacterThing:
    get:
        return character_select.thing as CharacterThing

var categorized_parts = {}
var selected_parts = []
var available_slots = []
var current_slot_index = 0

var character_preview_texture
@export var character_camera: Camera3D

func _exit_tree():
    if character != null:
        character.queue_free()

func set_text():
    if character_text != null:
        character_text.text = "%s - %d/%d" % [available_slots[current_slot_index].thing_type, current_slot_index + 1, len(available_slots)]

func categorize_character_parts():
    for part: PackedScene in character_part_list.character_parts:
        var part_thing = part.instantiate() as CharacterPartThing
        if part_thing != null:
            var category = part_thing.thing_type
            if not categorized_parts.has(category):
                categorized_parts[category] = []
            categorized_parts[category].append(part)
            part_thing.queue_free()

func initialize_selected_parts():
    selected_parts = [available_slots.size()]

func update_selected_parts_list():
    var new_size = len(available_slots)
    if len(selected_parts) < new_size:
        for i in range(len(selected_parts), new_size):
            selected_parts.append(0)
    elif len(selected_parts) > new_size:
        selected_parts = selected_parts[new_size]

func update_character():
    character.character_parts = []
    for i in range(len(available_slots)):
        var slot = available_slots[i]
        var thing_type = slot.thing_type
        var parts = categorized_parts[thing_type]
        var index = selected_parts[i]
        if parts[index] != null and index > 0 and parts[index].has_node("CharacterPartThing"):
            var part_thing = parts[index].get_node("CharacterPartThing")
            character.character_parts.append(part_thing.character_part_info)
    character.assemble_character()
    available_slots = character.get_character_part_slots()
    update_selected_parts_list()
    set_text()

func move(direction: Vector2):
    if abs(direction.x) > abs(direction.y):
        var thing_type = available_slots[current_slot_index].thing_type
        selected_parts[current_slot_index] += direction.x
        var part_count = len(categorized_parts[thing_type])
        selected_parts[current_slot_index] = ((selected_parts[current_slot_index] % part_count) + part_count) % part_count
    elif abs(direction.x) < abs(direction.y):
        current_slot_index -= direction.y
        current_slot_index = clamp(current_slot_index, 0, len(available_slots) - 1)

    update_character()

func primary(pressed: bool):
    if pressed:
        if character_camera != null:
            character.thing_portrait = character_camera.get_viewport().get_texture()

        # Save the character
        character.save_thing("Player")

        # Load the characters
        character_select.get_characters()

        # Return to the character selection screen
        visible = false

func secondary(pressed: bool):
    if pressed:
        # Return to the character selection screen
        visible = false

func _on_visibility_changed() -> void:
    if visible:
        character.thing_name = character_select.new_character_name
        categorize_character_parts()
        if character == null:
            print("CharacterThing is null")
            return

        available_slots = character.get_thing_slots()
        initialize_selected_parts()
        set_text()
