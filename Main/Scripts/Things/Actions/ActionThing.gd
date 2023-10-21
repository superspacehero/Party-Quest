extends UnsavedThing
class_name ActionThing

# Variables and properties
var user: GameThing # Assuming GameThing is a Node
var action_running: bool = false
var action_successful: bool = false  # To track whether the action was completed successfully or not

# Signals
signal action_end
signal action_complete

func get_thing_type() -> String:
    return "Action"
func get_thing_subtype() -> String:
    return "Action"

func use(_user: GameThing):
    if not action_running:
        user = _user
        action_running = true
        user.action_list.set_action(self)
        self.show()
        await run_action()  # Ensure we wait for run_action to finish


func run_action() -> void:
    while action_running:
        await action_complete  # Directly await the signal, no need to connect it to a method

    finalize_action()

func finalize_action() -> void:
    emit_signal("action_end")
    emit_signal("action_complete")

    user.action_list.clear_action()
    user.action_list.used_action_categories.append(self.thing_subtype)

    # If the user is of type CharacterThing, do something
    # You may have to define or replace the methods and types as per your actual logic
    if user is CharacterThing:
        (user as CharacterThing).display_actions(true)

    set_process(false)

func end_action() -> void:
    action_successful = true
    action_running = false

func cancel_action():
    action_successful = false
    action_running = false

    # If the user is of type CharacterThing, do something
    # You may have to define or replace the methods and types as per your actual logic
    #if user is CharacterThing:
    #	user.display_actions(true)
    #	print("ActionThing %s was cancelled." % _thing_name) # Uncomment if needed

    self.hide()

func _on_Timer_timeout():
    pass
