extends ActionThing
class_name EndAction

# Override thingSubType
func get_thing_subtype() -> String:
	return "End"

# Override run_action
func run_action() -> void:
	user.occupy_current_node()
	GameManager.next_character()
	
	# End the action
	end_action()
	finalize_action()