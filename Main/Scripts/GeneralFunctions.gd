extends Node

# Function to get all children of a node
func get_all_children(in_node, arr := []):
	arr.push_back(in_node)
	for child in in_node.get_children():
		arr = get_all_children(child, arr)
	return arr

# Function to find all nodes with a specific class
func find_by_class(node: Node, target_class: String) -> Array:
	var result = []
	var all_children = get_all_children(node)
	for child in all_children:
		if child.is_class(target_class):
			result.push_back(child)
			print_debug("Found ", target_class, " in ", child)
	return result

# Function to find all GameThings with a specific type
func find_all_by_type(node: Node, target_type: String) -> Array:
	var result = []
	var all_children = get_all_children(node)
	for child in all_children:
		if child.has_method("get_thing_type"):
			if child.get_thing_type() == target_type:
				result.push_back(child)
				print_debug("Found ", target_type, " in ", child)
	return result

# Function to find an individual GameThing with a specific type
func find_by_type(node: Node, target_type: String) -> Node:
	var all_children = get_all_children(node)
	for child in all_children:
		if child.has_method("get_thing_type"):
			if child.get_thing_type() == target_type:
				return child
	return null