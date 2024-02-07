extends UnsavedThing
class_name CharacterSelect

var initialized: bool
var can_add_new_character: bool
var can_add_extra_players: bool = true
@export var new_character_name: String
@export var new_character_sprite: Sprite2D

@export var extra_character_select_prefab: PackedScene

@export_range(0.0, 1.0) var input_deadzone: float = 0.75
@export var selected_overlay: Node
@export var selected_checkmark: Node
@export var add_player_button: Node

@export var to_new_character_arrow: Node
@export var character_select_arrows: Node
@export var to_character_select_arrow: Node

var character: Node

var thing_input: ThingInput
var new_character: CharacterInfo
var characters: Array
var selected_characters: Array
var characters_loaded: bool

var selected_character: CharacterInfo:
    set(value):
        if characters_loaded:
            update_arrow_states(value == null)


var character_ui: CharacterUI

var new_character_selected: bool

var character_index: int
var current_character: CharacterInfo

func _ready():
    if CharacterSelectMenu.instance != null:
        CharacterSelectMenu.instance.character_selects.append(self)

    selected_character = null
    add_player_button.visible = can_add_extra_players

func _on_disabled():
    if CharacterSelectMenu.instance != null:
        CharacterSelectMenu.instance.character_selects.erase(self)

func _start():
    await initialize()

func initialize():
    if not characters_loaded:
        characters_loaded = true

        while characters == null:
            LoadCharacters()

            print("Found", characters.size(), "characters")

            new_character = CharacterInfo.new()
            new_character.name = new_character_name
            new_character.sprite = new_character_sprite
            characters.append(new_character)

    await get_tree().process_frame

    initialized = true
    if thing_input != null:
        thing_input.can_control = false

    if CharacterSelectMenu.instance != null:
        set_parent(CharacterSelectMenu.instance.character_select_parent, true)

    new_character_selected = characters.size() == 1

func load_characters():
    characters = CharacterThing.CharacterInfo.LoadCharacters("Player")

    if CharacterSelectMenu.instance != null:
        for character_select in CharacterSelectMenu.instance.character_selects:
            character_select.UpdateCharacterSelect()

func update_character_select():
    if not characters_loaded:
        return

    if characters.size() > 0:
        character_index = characters.index(current_character)

func move(direction: Vector2):
    if direction.normalized().length() < input_deadzone:
        direction = Vector2.ZERO

    var new_input_direction = direction.round()

    if thing_input != null and thing_input.is_active():
        if new_input_direction != input_direction:
            thing_input.move(new_input_direction)
    elif extra_select != null:
        extra_select.move(new_input_direction)
    else:
        if not characters_loaded or not initialized:
            return

        if new_input_direction != input_direction:
            input_direction = new_input_direction

            if selected_character != null:
                return

            if abs(input_direction.x) > abs(input_direction.y):
                if not new_character_selected:
                    character_index += input_direction.x
            elif abs(input_direction.x) < abs(input_direction.y):
                if can_add_new_character:
                    if input_direction.y > 0:
                        new_character_selected = true
                    elif input_direction.y < 0:
                        new_character_selected = false

func move_horizontal(direction: int):
    move(Vector2(direction, 0))
    move(Vector2.ZERO)

func move_vertical(direction: int):
    move(Vector2(0, direction))
    move(Vector2.ZERO)

func primary(pressed: bool):
    if thing_input != null and thing_input.is_active():
        thing_input.primary(pressed)
    elif extra_select != null:
        extra_select.primary(pressed)
    else:
        if not characters_loaded or not initialized:
            return

        if not is_visible():
            show()
            return

        if pressed:
            if new_character_selected:
                if can_add_new_character:
                    if thing_input != null:
                        thing_input.show()
            else:
                if CharacterSelectMenu.instance != null and CharacterSelectMenu.instance.all_character_selects_ready:
                    CharacterSelectMenu.instance.select_next_menu()
                    return

                selected_character = characters[character_index]

                selected_characters.append(selected_character)

                UpdateCharacterSelect()

func primary_press():
    primary(true)
    primary(false)

func secondary(pressed: bool):
    if thing_input != null and thing_input.is_active():
        thing_input.secondary(pressed)
    elif extra_select != null:
        extra_select.secondary(pressed)
    else:
        if not characters_loaded or not initialized:
            return

        if pressed:
            if selected_character != null:
                if not new_character_selected:
                    selected_characters.remove(selected_character)

                selected_character = null

                CharacterSelectMenu.instance.check_all_character_selects_ready()
            else:
                var looped_character_select = self
                while looped_character_select.extra_select != null:
                    looped_character_select = looped_character_select.extra_select

                if CharacterSelectMenu.instance != null:
                    CharacterSelectMenu.instance.character_selects.remove(looped_character_select)

                looped_character_select.character.queue_free()
                looped_character_select.queue_free()

func secondary_press():
    secondary(true)
    secondary(false)

func tertiary(pressed: bool):
    if thing_input != null and thing_input.is_active():
        thing_input.tertiary(pressed)
    else:
        if pressed and selected_character != null and can_add_extra_players:
            if CharacterSelectMenu.instance != null:
                var looped_character_select = self
                while looped_character_select.extra_select != null:
                    looped_character_select = looped_character_select.extra_select

                var character_select = extra_character_select_prefab.instance()
                if character_select != null:
                    character_select.player_input = player_input
                    looped_character_select.extra_select = character_select

func tertiary_press():
    tertiary(true)
    tertiary(false)

func update_arrow_states(showArrows: bool = true) -> void:
    if to_new_character_arrow != null:
        to_new_character_arrow.visible = (showArrows and not _newCharacterSelected)

    if character_select_arrows != null:
        character_select_arrows.set_active(showArrows and not _newCharacterSelected)

    if to_character_select_arrow != null:
        to_character_select_arrow.set_active(showArrows and _newCharacterSelected)
