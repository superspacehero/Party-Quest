extends GridMap

func _ready():
    # enable navigation mesh for grid items
    set_bake_navigation(true)

    # get grid items, create and set a new navigation mesh for each item in the MeshLibrary
    var gridmap_item_list: PackedInt32Array = mesh_library.get_item_list()
    for item in gridmap_item_list:
        var new_item_navigation_mesh: NavigationMesh = NavigationMesh.new()
        # # Add vertices and polygons that describe the traversable ground surface.
        # # E.g. for a convex polygon that resembles a flat square.
        # new_item_navigation_mesh.vertices = PackedVector3Array([
        #     Vector3(-0.5, 0.0, 0.5),
        #     Vector3(0.5, 0.0, 0.5),
        #     Vector3(0.5, 0.0, -0.5),
        #     Vector3(-0.5, 0.0, -0.5),
        # ])
        # new_item_navigation_mesh.add_polygon(
        #     PackedInt32Array([0, 1, 2, 3])
        # )
        # mesh_library.set_item_navigation_mesh(item, new_item_navigation_mesh)
        # mesh_library.set_item_navigation_mesh_transform(item, Transform3D())

    # # clear the cells
    # clear()

    # # add procedual cells using the first item
    # var _position: Vector3i = Vector3i(global_transform.origin)
    # var _item: int = 0
    # var _orientation: int = 0
    # for i in range(0, 10):
    #     for j in range(0, 10):
    #         _position.x = i
    #         _position.z = j
    #         set_cell_item(_position, _item, _orientation)
    #         _position.x = -i
    #         _position.z = -j
    #         set_cell_item(_position, _item, _orientation)
