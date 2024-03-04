extends Menu
class_name CharacterSelectMenu

# Singleton
static var instance: CharacterSelectMenu = null

@export var character_select_parent: Control:
    get:
        if character_select_parent == null:
            character_select_parent = self
        return character_select_parent

var character_selects: Array[CharacterSelect] = []

var all_character_selects_ready: bool:
    get:
        all_character_selects_ready = true
        for character_select in character_selects:
            if character_select.selected_character == null:
                all_character_selects_ready = false
                break
        return all_character_selects_ready

func select_next_menu():
    # Save all players and their selected characters
    GameManager.players.clear()

    for character_select in character_selects:
        GameManager.players.append(GameManager.PlayerAndCharacter.new(character_select.player, character_select.selected_character))

    super.select_next_menu()

func select():
    super.select()

    instance = self

    if InputManager.instance != null:
        InputManager.instance.allow_joining = true

func deselect():
    super.deselect()

    instance = null

    if InputManager.instance != null:
        InputManager.instance.allow_joining = false

    # Reset all character selects
    for character_select in character_selects:
        character_select.queue_free()

    character_selects.clear()