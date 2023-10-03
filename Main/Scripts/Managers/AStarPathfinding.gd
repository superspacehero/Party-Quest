extends Node3D
class_name AStarPathfinding


@export var should_draw_cubes : bool = false
@export var minimum_gap_size: float = 2.0
@export var gridmap: GridMap

var grid_step = 1.0
var points = {}
var astar = AStar3D.new()

var cube_mesh = BoxMesh.new()
var red_material = StandardMaterial3D.new()
var green_material = StandardMaterial3D.new()


func _ready():
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
		_add_highest_pathables_for(cell_position.x, cell_position.z, cell_size.y, highest_y)

func _get_highest_y_value() -> float:
	var highest_y = float("-inf")
	for cell in gridmap.get_used_cells():
		if cell.y > highest_y:
			highest_y = cell.y
	return highest_y * gridmap.cell_size.y


func _add_highest_pathables_for(x: float, z: float, step_y: float, max_y: float):
	var last_occupied_y = max_y + step_y
	var gap_start_y = last_occupied_y
	var is_gap_started = false
	
	for y in range(ceil(max_y / step_y), -1, -1):
		var world_pos = Vector3(x, y * step_y, z)
		var cell = gridmap.local_to_map(world_pos)
		
		if not is_cell_occupied(cell):
			if is_gap_started and (gap_start_y - world_pos.y) >= minimum_gap_size:
				_add_point(Vector3(x, last_occupied_y, z))
				last_occupied_y = world_pos.y
				is_gap_started = false
			elif not is_gap_started:
				gap_start_y = world_pos.y
				is_gap_started = true
		else:
			last_occupied_y = world_pos.y
			is_gap_started = false

func is_cell_occupied(cell: Vector3) -> bool:
	return gridmap.get_cell_item(cell) != -1

func _add_point(point: Vector3):
	var id = astar.get_available_point_id()

	astar.add_point(id, point)
	points[world_to_astar(point)] = id
	_create_nav_cube(point)
	print("Added point %s with id %d" % [point, id])


func _connect_points():
	for point in points:
		var pos_str = point.split(",")
		var world_pos := Vector3(pos_str[0], pos_str[1], pos_str[2])
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
			var search_offset = Vector3(x, 0, z)
			if search_offset == Vector3.ZERO:
				continue

			var potential_neighbor = world_to_astar(world_point + search_offset)
			if points.has(potential_neighbor):
				adjacent_points.append(points[potential_neighbor])
	return adjacent_points


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


func world_to_astar(world: Vector3) -> String:
	var x = snapped(world.x, grid_step)
	var y = snapped(world.y, grid_step)
	var z = snapped(world.z, grid_step)
	return "%d,%d,%d" % [x, y, z]

