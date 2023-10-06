@tool
extends GridMap
class_name AutoGridMap

@export var refresh: bool:
    set(value):
        set_refresh(value)

@export var mesh_library_3d: MeshLibrary

const Top = 0x01
const Bottom = 0x02
const Standalone = 0x04

var meshes: Dictionary
var sub_gridmap: GridMap

func set_refresh(value):
    print("Refreshed!")
    _setup()

func _ready():
    _setup()

func _setup():
    if mesh_library_3d == null:
        return
    
    sub_gridmap = get_node_or_null("subgridmap")
    
    if !sub_gridmap:
        sub_gridmap = GridMap.new()
        add_child(sub_gridmap)
        sub_gridmap.name = "subgridmap"
        sub_gridmap.cell_size = Vector3(1, 1, 1)
        sub_gridmap.mesh_library = mesh_library_3d
        sub_gridmap.set_owner(get_tree().edited_scene_root)
    
    # Create a function to fetch mesh by name to make the code cleaner
    
    meshes = {
        Top: { "mesh": get_mesh_item("Top"), "orientation": 0 },
        Bottom: { "mesh": get_mesh_item("Bottom"), "orientation": 16 },
        Top | Bottom: { "mesh": get_mesh_item("Middle"), "orientation": 0 },
        Standalone: { "mesh": get_mesh_item("Standalone"), "orientation": 0 }
    }

    # Optional: You may want to automatically update the grid after setting up
    _update_grid()

func get_mesh_item(mesh_name: String):
    mesh_library_3d.find_item_by_name(mesh_name)

func _get_neighborhood_mask(cell_position: Vector3) -> int:
    var mask = 0x00

    # Check for each neighboring position and adjust the mask accordingly
    if sub_gridmap.get_cell_item(Vector3(cell_position.x, cell_position.y + 1, cell_position.z)) != -1:
        mask |= Top
    if sub_gridmap.get_cell_item(Vector3(cell_position.x, cell_position.y - 1, cell_position.z)) != -1:
        mask |= Bottom

    # If neither Top nor Bottom neighbors are present, it's a standalone cell
    if mask == 0x00:
        mask |= Standalone

    return mask

func _update_grid():
    # Loop over each tile in the grid
    for cell in get_used_cells():
        # Get the position of the tile
        var cell_position = cell
        
        # Detect the neighborhood for the current tile
        var mask = _get_neighborhood_mask(cell_position)
        
        # Based on the neighborhood, get the appropriate mesh and orientation
        var mesh_data = meshes.get(mask, null)
        
        if mesh_data:
            set_cell_item(cell_position, mesh_data["mesh"])
        else:
            set_cell_item(cell_position, -1)

        print("Cell: ", cell_position, "Mask: ", mask)

func _on_sub_gridmap_updated():
    _update_grid()
