extends Menu
class_name CharacterSelectMenu

# Singleton
static var instance: CharacterSelectMenu = null

@export var character_select_parent: Node:
    get:
        if character_select_parent == null:
            character_select_parent = self
        return character_select_parent

@export var character_select_scene: PackedScene = null

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

func deselect():
    super.deselect()

    instance = null

    # Reset all character selects
    for character_select in character_selects:
        character_select.queue_free()

    character_selects.clear()

func primary(pressed):
    # if first_press:
    #     first_button_press()
    #     return
    if input_ready and pressed:
        for input in InputManager.instance.get_inputs():
            if input.primary_action:
                for character_select in character_selects:
                    if input.inventory.has(character_select):
                        return

                var new_character_select = character_select_scene.instantiate()
                character_select_parent.add_child(new_character_select)
                new_character_select = new_character_select as CharacterSelect
                character_selects.append(new_character_select)
                input.inventory.append(new_character_select)
                print("Inputs: " + str(input.inventory))

func secondary(pressed):
    # if first_press:
    #     first_button_press()
    #     return
    if input_ready and pressed:
        if character_selects.size() == 0:
            select_previous_menu()