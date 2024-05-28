extends Node3D
class_name NodeFollower

@export var nodes_to_follow: Array = [Node3D]

func _process(_delta):
	if nodes_to_follow.size() > 0:
		var average_position = nodes_to_follow[0].global_position
		if nodes_to_follow.size() > 1:
			for i in range(1, nodes_to_follow.size()):
				average_position += nodes_to_follow[i].global_position
			average_position /= nodes_to_follow.size()

		global_position = average_position