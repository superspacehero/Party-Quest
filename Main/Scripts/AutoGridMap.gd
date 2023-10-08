@tool
extends GridMap
class_name AutoGridMap

@export var refresh: bool:
    set(value):
        set_refresh(value)

@export var mesh_library_3d: MeshLibrary

const Top = 0x01
const Bottom = 0x02
const Solo = 0x04

var meshes: Dictionary

func set_refresh(_value):
    print("Refreshed!")
    _setup()

func _ready():
    _setup()

func _setup():
    if mesh_library_3d == null:
        return

    meshes = {
        Top: [],
        Bottom: [],
        Top | Bottom: [],
        Solo: []
    }

    for i in range(mesh_library_3d.get_item_list().size()):
        var item_name = mesh_library_3d.get_item_name(i)
        var mesh_data = { "mesh": i, "orientation": 0 } # default orientation

        if item_name.ends_with("Top"):
            meshes[Top].append(mesh_data)
        elif item_name.ends_with("Bot"):
            meshes[Bottom].append(mesh_data)
        elif item_name.ends_with("Mid"):
            meshes[Top | Bottom].append(mesh_data)
        elif item_name.ends_with("Sol"):
            meshes[Solo].append(mesh_data)

    _update_grid()

func _get_neighborhood_mask(cell_position: Vector3) -> int:
    var mask = 0x00
    var current_tile_name = mesh_library_3d.get_item_name(get_cell_item(cell_position))
    var prefix = current_tile_name.substr(0, current_tile_name.length() - 3)

    var top_neighbor = _is_neighboring_tile_matching(Vector3(cell_position.x, cell_position.y - 1, cell_position.z), prefix)
    var bottom_neighbor = _is_neighboring_tile_matching(Vector3(cell_position.x, cell_position.y + 1, cell_position.z), prefix)

    if top_neighbor: 
        mask |= Top
    if bottom_neighbor: 
        mask |= Bottom
    if !top_neighbor and !bottom_neighbor:
        mask |= Solo

    return mask

func _is_neighboring_tile_matching(tile_position, prefix):
    var neighboring_tile_id = get_cell_item(tile_position)
    if neighboring_tile_id != -1:
        return mesh_library_3d.get_item_name(neighboring_tile_id).begins_with(prefix)
    return false

func _update_grid(specific_coord = null):
    if specific_coord:
        # Check and update the tiles upwards
        var current_position = specific_coord
        while get_cell_item(current_position) != -1:
            _update_cell(current_position)
            current_position.y += 1  # Move upwards

        # Reset position and check downwards
        current_position = specific_coord
        current_position.y -= 1  # Start below the given cell
        while get_cell_item(current_position) != -1:
            _update_cell(current_position)
            current_position.y -= 1  # Move downwards

    else:
        # Original loop to update all cells
        for cell in get_used_cells():
            _update_cell(cell)

func _update_cell(cell_position: Vector3):
    var mask = _get_neighborhood_mask(cell_position)
    var current_tile_name = mesh_library_3d.get_item_name(get_cell_item(cell_position))
    var prefix = current_tile_name.substr(0, current_tile_name.length() - 3)

    if mask in meshes:
        for mesh_data in meshes[mask]:
            if mesh_library_3d.get_item_name(mesh_data["mesh"]).begins_with(prefix):
                set_cell_item(cell_position, mesh_data["mesh"])
                break


func get_mesh_id_from_prefix_and_suffix(prefix: String, suffix: String) -> int:
    for i in range(mesh_library_3d.get_item_list().size()):
        var item_name = mesh_library_3d.get_item_name(i)
        if item_name == prefix + suffix:
            return i
    return -1
