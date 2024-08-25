extends ThingUI
class_name CharacterSelect

var initialized: bool

@export var character_category: String = "Player"
@export var _can_add_extra_players: bool = true

@export_category("Animation")
@export var character_selected_animator: AnimationPlayer
@export var character_selected_animation: String = "Ready"

@export_category("New Characters")
@export var character_creator: CharacterCreator
@export var new_character_name: String
@export var new_character_texture: Texture2D

@export_category("Input")
@export_range(0.0, 1.0) var input_deadzone: float = 0.75
@export var selected_overlay: Node
@export var selected_checkmark: Node
@export var add_player_button: Node

@export_category("Arrows")
@export var to_new_character_arrow: Node
@export var character_select_arrows: Node
@export var to_character_select_arrow: Node

var input: ThingInput
var extra_select: CharacterSelect

var characters:
    get:
        return CharacterSelectMenu.instance.characters

var new_character: CharacterThing
var selected_characters: Array
var characters_loaded: bool:
    get:
        return characters != null

var can_add_extra_players: bool:
    get:
        return _can_add_extra_players and selected_character != null

var selected_character: CharacterThing:
    set(value):
        if characters_loaded:
            update_arrow_states(value == null)

            if character_selected_animator != null:
                if value != null:
                    character_selected_animator.play(character_selected_animation)
                else:
                    character_selected_animator.play_backwards(character_selected_animation)
            
            update_character_select()

var new_character_selected: bool:
    set(value):
        new_character_selected = character_creator && value

        if not characters_loaded:
            update_arrow_states(false)
            return

        update_arrow_states()

        current_character = new_character if new_character_selected else characters[character_index]

var character_index: int:
    set(value):
        # Make sure the value wraps from to the number of characters
        value = value % characters.size()

        if not new_character_selected:
            character_index = value

        if characters_loaded:
            if characters.size() <= 0:
                new_character_selected = true
            
            current_character = new_character if new_character_selected else characters[character_index]

var current_character: CharacterThing:
    set(value):
        if initialized and characters_loaded and value != null:
            current_character = value

func _ready():
    new_character = CharacterSelectMenu.instance.character_scene.instantiate()
    new_character.thing_name = new_character_name
    new_character.thing_portrait = new_character_texture

    await get_tree().process_frame

    initialized = true
    new_character_selected = characters.size() <= 0

    update_character_select()

func update_character_select():
    if not characters_loaded:
        return

    add_player_button.visible = can_add_extra_players

    if characters.size() > 0:
        character_index = character_index % characters.size()

    if CharacterSelectMenu.instance != null:
        CharacterSelectMenu.instance.check_all_character_selects_ready()

func move(direction):
    if not initialized:
        return

    if direction.normalized().length() < input_deadzone:
        direction = Vector2.ZERO

    # print("CharacterSelect.move: " + str(direction))

    var new_input_direction = direction.round()

    if extra_select != null:
        extra_select.move(new_input_direction)
    else:
        if not characters_loaded or not initialized:
            return

        if new_input_direction != direction:
            direction = new_input_direction

            if selected_character != null:
                return

            if abs(direction.x) > abs(direction.y):
                if not new_character_selected:
                    character_index += round(direction.x)
            elif abs(direction.x) < abs(direction.y):
                if character_creator:
                    if direction.y > 0:
                        new_character_selected = true
                    elif direction.y < 0:
                        new_character_selected = false

func move_horizontal(direction: int):
    move(Vector2(direction, 0))
    move(Vector2.ZERO)

func move_left():
    move_horizontal(-1)

func move_right():
    move_horizontal(1)

func move_vertical(direction: int):
    move(Vector2(0, direction))
    move(Vector2.ZERO)

func move_up():
    move_vertical(-1)

func move_down():
    move_vertical(1)

func set_character_creator_visibility(visiblity: bool):
    if character_creator != null:
        character_creator.visible = visiblity
        update_arrow_states()


func primary(pressed):
    if not initialized:
        return

    if character_creator != null and character_creator.is_visible():
        character_creator.primary(pressed)
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
                if character_creator:
                    set_character_creator_visibility(true)
            else:
                if CharacterSelectMenu.instance != null and CharacterSelectMenu.instance.all_character_selects_ready:
                    CharacterSelectMenu.instance.select_next_menu()
                    return

                selected_character = characters[character_index]

                selected_characters.append(selected_character)

            update_character_select()

func primary_press():
    primary(true)
    primary(false)

func secondary(pressed):
    if not initialized:
        return

    if character_creator != null and character_creator.is_visible():
        character_creator.secondary(pressed)
    elif extra_select != null:
        extra_select.secondary(pressed)
    else:
        if pressed:
            if not characters_loaded:
                print("Characters not loaded")
                return
            if not initialized:
                print("Not initialized")
                return

            if extra_select != null:
                var looped_character_select = self
                while looped_character_select.extra_select != null:
                    looped_character_select = looped_character_select.extra_select

                looped_character_select.destroy()
            elif selected_character != null:
                if not new_character_selected:
                    selected_characters.erase(selected_character)

                selected_character = null
            else:
                if new_character_selected:
                    if character_creator:
                        set_character_creator_visibility(false)
                else:
                    if CharacterSelectMenu.instance != null:
                        CharacterSelectMenu.instance.character_selects.erase(self)

                    destroy()

            update_character_select()

func secondary_press():
    secondary(true)
    secondary(false)

func tertiary(pressed):
    if not initialized:
        return

    if pressed and can_add_extra_players:
        if CharacterSelectMenu.instance != null:
            var looped_character_select = self
            while looped_character_select.extra_select != null:
                looped_character_select = looped_character_select.extra_select

            var character_select = CharacterSelectMenu.instance.add_character_select(null)
            if character_select != null:
                looped_character_select.extra_select = character_select

        update_character_select()

func tertiary_press():
    tertiary(true)
    tertiary(false)

func pause(pressed):
    if not initialized:
        return

    if pressed:
        if character_creator != null and character_creator.is_visible():
            character_creator.pause(pressed)
        else:
            if extra_select != null:
                extra_select.pause(pressed)

        update_character_select()

func update_arrow_states(show_arrows: bool = true) -> void:
    print("CharacterSelect.update_arrow_states: " + str(show_arrows))

    if to_new_character_arrow != null:
        to_new_character_arrow.visible = character_creator.visible or (show_arrows and not new_character_selected)

    if character_select_arrows != null:
        character_select_arrows.visible = character_creator.visible or (show_arrows and not new_character_selected)

    if to_character_select_arrow != null:
        to_character_select_arrow.visible = character_creator.visible or (show_arrows and new_character_selected)

func destroy():
    if selected_character != null:
        selected_character = null
    else:
        if CharacterSelectMenu.instance != null:
            CharacterSelectMenu.instance.character_selects.erase(self)
        queue_free()