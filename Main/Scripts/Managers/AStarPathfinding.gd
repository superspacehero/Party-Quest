extends Node3D
class_name AStarPathfinding

static var instance: AStarPathfinding

@export var should_draw_cubes : bool = false
@export var minimum_gap_size: float = 2.0
@export var gridmap: GridMap
@export var point_display: ObjectPool

@export_group("Colors")
@export var walkable_color: Color = Color.WHITE
@export var current_color: Color = Color.BLUE
@export var occupied_color: Color = Color.RED

var grid_step = 1.0
var points = {}
var astar = AStar3D.new()

var cube_mesh = BoxMesh.new()
var red_material = StandardMaterial3D.new()
var green_material = StandardMaterial3D.new()

func _ready():
	instance = self

	red_material.albedo_color = Color.RED
	green_material.albedo_color = Color.GREEN
	cube_mesh.size = Vector3(0.25, 0.25, 0.25)
	_add_points()
	_connect_points()

func _add_points():
	var cell_size = gridmap.cell_size
	var highest_y = _get_highest_y_value()
	for cell in gridmap.get_used_cells():
		var cell_position = gridmap.map_to_local(cell)
		_add_highest_pathables_for(cell_position.x, cell_position.z, cell_size.y, highest_y, -highest_y)

func _get_highest_y_value() -> float:
	var highest_y = float("-inf")
	for cell in gridmap.get_used_cells():
		if cell.y > highest_y:
			highest_y = cell.y
	highest_y += minimum_gap_size
	return highest_y * gridmap.cell_size.y


func _add_highest_pathables_for(x: float, z: float, step_y: float, max_y: float, min_y: float):
	var gap_size = 0.0
	# var gap_start_y = 0.0
	var is_gap_started = false

	for y in range(ceil(max_y / step_y), floor(min_y / step_y), -1):
		var world_pos = Vector3(x, y * step_y, z)
		var cell = gridmap.local_to_map(world_pos)

		if not is_cell_occupied(cell):
			if not is_gap_started:
				is_gap_started = true
			gap_size += step_y
		else:
			if is_gap_started and gap_size >= minimum_gap_size:
				_add_point(Vector3(x, y, z))
			gap_size = 0.0
			is_gap_started = false


func is_cell_occupied(cell: Vector3) -> bool:
	return gridmap.get_cell_item(cell) != -1

func _add_point(point: Vector3):
	var id = astar.get_available_point_id()

	astar.add_point(id, point)
	points[world_to_astar(point)] = id
	_create_nav_cube(point)
	# print("Added point %s with id %d" % [point, id])


func _connect_points():
	for point in points:
		var world_pos : Vector3 = point
		var adjacent_points = _get_adjacent_points(world_pos)
		var current_id = points[point]
		for neighbor_id in adjacent_points:
			if not astar.are_points_connected(current_id, neighbor_id):
				astar.connect_points(current_id, neighbor_id)
				if should_draw_cubes:
					get_child(current_id).material_override = green_material
					get_child(neighbor_id).material_override = green_material


func _create_nav_cube(cube_position: Vector3):
	if should_draw_cubes:
		var cube = MeshInstance3D.new()
		cube.mesh = cube_mesh
		cube.material_override = red_material
		add_child(cube)
		cube.global_position = cube_position


func _get_adjacent_points(world_point: Vector3) -> Array:
	var adjacent_points = []
	var search_coords = [-grid_step, 0, grid_step]

	for x in search_coords:
		for z in search_coords:
			# Check same level, one level up and down.
			for y_offset in range(0, -2, -1):  # 0 for same level, -1 for one below, -2 for two below etc.
				if y_offset == 0 and x == 0 and z == 0:
					continue  # Skip checking the node against itself

				var search_offset = Vector3(x, y_offset * grid_step, z)
				var potential_neighbor = world_to_astar(world_point + search_offset)

				if points.has(potential_neighbor):
					adjacent_points.append(points[potential_neighbor])
	return adjacent_points

func check_point_occupied(point: Vector3) -> bool:
	var astar_id = points[point]
	return astar.is_point_disabled(astar_id)

func occupy_point(point):
	astar.set_point_disabled(point, true)
	if should_draw_cubes:
		get_child(point).material_override = red_material

func unoccupy_point(point):
	astar.set_point_disabled(point, false)
	if should_draw_cubes:
		get_child(point).material_override = green_material

func handle_obstacle_added(obstacle: Node3D):
	var normalized_origin = obstacle.global_transform.origin

# Uncomment this line if you want to disable/enabled adjacent points
# of an obstacle.
#	var adjacent_points: Array = _get_adjacent_points(normalized_origin)
	var adjacent_points: Array = []
	var point_key = world_to_astar(normalized_origin)
	var astar_id = points[point_key]
	adjacent_points.append(astar_id)

	for point in adjacent_points:
		if not astar.is_point_disabled(point):
			astar.set_point_disabled(point, true)
			if should_draw_cubes:
				get_child(point).material_override = red_material


func handle_obstacle_removed(obstacle: Node3D):
	var normalized_origin = obstacle.global_transform.origin

# Uncomment this line if you want to disable/enabled adjacent points
# of an obstacle.
#	var adjacent_points: Array = _get_adjacent_points(normalized_origin)
	var adjacent_points: Array = []
	var point_key = world_to_astar(normalized_origin)
	var astar_id = points[point_key]
	adjacent_points.append(astar_id)

	for point in adjacent_points:
		if astar.is_point_disabled(point):
			astar.set_point_disabled(point, false)
			if should_draw_cubes:
				get_child(point).material_override = green_material


func find_path(from: Vector3, to: Vector3) -> Array:
	var start_id = astar.get_closest_point(from)
	var end_id = astar.get_closest_point(to)
	return astar.get_point_path(start_id, end_id)


func world_to_astar(world: Vector3) -> Vector3:
	var x = snapped(world.x, grid_step)
	var y = snapped(world.y, grid_step)
	var z = snapped(world.z, grid_step)
	return Vector3(x, y, z)

func is_walkable(astar_point: Vector3) -> bool:
	var astar_id = points[astar_point]
	return not astar.is_point_disabled(astar_id)

func astar_to_world(astar_point: Vector3) -> Vector3:
	var x = astar_point.x * grid_step
	var y = astar_point.y * grid_step
	var z = astar_point.z * grid_step
	return Vector3(x, y, z)

func get_points_in_radius(center: Vector3, radius: float, max_height_limits: Vector2) -> Array:
	var points_in_radius = []

	if max_height_limits.x < 0:
		max_height_limits.x = float("inf")
	if max_height_limits.y < 0:
		max_height_limits.y = float("inf")

	# If radius is set to infinity, add all walkabale points to the points_in_radius array.
	if radius == float("inf"):
		for point in points:
			var world_pos = astar_to_world(point)
			if world_pos.y >= max_height_limits.x and world_pos.y <= max_height_limits.y:
				points_in_radius.append(world_pos)
		return points_in_radius
	else:
		var current_point = world_to_astar(center)
		points_in_radius.append(current_point)

		var queue = []
		queue.append(current_point)
		
		var directions = [Vector3(1, 0, 0), Vector3(-1, 0, 0), Vector3(0, 0, 1), Vector3(0, 0, -1)]

		# Iterate through all the points in the movement range
		for i in range(int(radius)):
			var queue_count = queue.size()
			for j in range(queue_count):
				var search_point = queue.pop_front()
				
				# Iterate through all the directions
				for direction in directions:
					var next_point = search_point + direction
					var next_world_pos = astar_to_world(next_point)

					if next_world_pos and not points_in_radius.has(next_point):
						# Check if the point is walkable
						if is_walkable(next_point):
							# Calculate the difference in height
							var delta_y = next_world_pos.y - astar_to_world(search_point).y

							if delta_y <= max_height_limits.x and delta_y >= -max_height_limits.y:
								points_in_radius.append(next_point)
								queue.append(next_point)
							
		return points_in_radius

# func color_node_object:
