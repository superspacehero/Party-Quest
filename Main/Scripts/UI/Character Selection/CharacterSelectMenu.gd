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

    if InputManager.instance != null:
        InputManager.instance.allow_joining = true
        InputManager.instance.device_joined.connect(connect_to_device)
        connect_to_all_inputs()

func deselect():
    super.deselect()

    instance = null

    if InputManager.instance != null:
        InputManager.instance.allow_joining = false
        if InputManager.instance.device_joined.get_connections().size() > 0:
            InputManager.instance.device_joined.disconnect(connect_to_device)

    # Reset all character selects
    for character_select in character_selects:
        character_select.queue_free()

    character_selects.clear()

func connect_to_all_inputs():
    if InputManager.instance != null:
        for input in InputManager.instance.get_inputs():
            connect_to_input(input as ThingInput)
    else:
        print("InputManager is null")

func connect_to_input(input: ThingInput):
    if input != null:
        if !input.inventory.has(self):
            input.inventory.append(self)
            # print("Inputs: " + str(input.inventory))

func connect_to_device(device: int):
    if InputManager.instance != null:
        var input = InputManager.instance.get_input(device)
        connect_to_input(input)

func primary(pressed):
    if pressed:
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
    if pressed:
        if character_selects.size() == 0:
            select_previous_menu()