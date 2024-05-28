@tool
extends EditorPlugin

enum OPERATION_TYPES {BIN_TO_TEXT, TEXT_TO_BIN}
@export var keep_originals = true

func _enter_tree():
    var file_system_dock = get_editor_interface().get_file_system_dock()
    connect_file_system_context_actions(file_system_dock)

func _exit_tree():
    var file_system_dock = get_editor_interface().get_file_system_dock()
    disconnect_file_system_context_actions(file_system_dock)

func connect_file_system_context_actions(file_system: FileSystemDock) -> void:
    file_system.files_moved.connect(_on_files_moved)
    file_system.file_removed.connect(_on_file_removed)
    file_system.folder_moved.connect(_on_folder_moved)
    file_system.folder_removed.connect(_on_folder_removed)
    file_system.display_mode_changed.connect(_on_display_mode_changed)

    # Ensure the context menu is connected to show the custom actions
    file_system.gui_input

func disconnect_file_system_context_actions(file_system: FileSystemDock) -> void:
    file_system.files_moved.disconnect(_on_files_moved)
    file_system.file_removed.disconnect(_on_file_removed)
    file_system.folder_moved.disconnect(_on_folder_moved)
    file_system.folder_removed.disconnect(_on_folder_removed)
    file_system.display_mode_changed.disconnect(_on_display_mode_changed)
    file_system.gui_input.disconnect(_on_file_system_dock_gui_input)

func _on_file_system_dock_gui_input(event):
    if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_RIGHT and event.pressed:
        var file_system_dock = get_editor_interface().get_file_system_dock()
        var context_menu = file_system_dock.get_popup()
        context_menu.about_to_popup.connect(Callable(self, "_on_context_menu_about_to_show"))
        print("Right click")

func _on_context_menu_about_to_show():
    var file_system_dock = get_editor_interface().get_file_system_dock()
    var selected_paths = file_system_dock.get_selected_files()
    add_custom_context_actions(get_editor_interface().get_file_system_dock().get_popup(), selected_paths)

func add_custom_context_actions(context_menu: PopupMenu, selected_paths: PackedStringArray) -> void:
    # Remove previous custom items
    var item_count = context_menu.get_item_count()
    for i in range(item_count):
        var item_text = context_menu.get_item_text(i)
        if item_text in ["Convert .scn to .tscn", "Convert .tscn to .scn"]:
            context_menu.remove_item(i)

    context_menu.add_separator()

    if selected_paths.size() == 0:
        context_menu.add_item("No file selected")
        return

    for path in selected_paths:
        if DirAccess.dir_exists_absolute(path):
            context_menu.add_item("Directory selected: " + path)
        elif FileAccess.file_exists(path):
            context_menu.add_item("Convert .scn to .tscn", OPERATION_TYPES.BIN_TO_TEXT)
            context_menu.add_item("Convert .tscn to .scn", OPERATION_TYPES.TEXT_TO_BIN)
            context_menu.id_pressed.connect(Callable(self, "_on_context_menu_option_pressed").bind(path))

func _on_context_menu_option_pressed(id, file_path):
    print("Option pressed with file path: ", file_path)
    if id == OPERATION_TYPES.BIN_TO_TEXT:
        if file_path.endswith(".scn") or file_path.endswith(".res"):
            convert_bin_to_text(file_path)
    elif id == OPERATION_TYPES.TEXT_TO_BIN:
        if file_path.endswith(".tscn"):
            convert_text_to_bin(file_path)

func convert_bin_to_text(file_path):
    var scene = load(file_path)
    if scene:
        var new_path = file_path.substr(0, file_path.length() - 4) + ".tscn"
        var err = ResourceSaver.save(new_path, scene)
        if err == OK:
            print("Saved: %s" % new_path)
            if not keep_originals:
                var dir = DirAccess.open("res://")
                dir.remove(file_path)
                print("Deleted: %s" % file_path)
        else:
            printerr("Error saving: %s" % new_path)
    else:
        printerr("Error loading: %s" % file_path)

func convert_text_to_bin(file_path):
    var scene = load(file_path)
    if scene:
        var new_path = file_path.substr(0, file_path.length() - 5) + ".scn"
        var err = ResourceSaver.save(new_path, scene)
        if err == OK:
            print("Saved: %s" % new_path)
            if not keep_originals:
                var dir = DirAccess.open("res://")
                dir.remove(file_path)
                print("Deleted: %s" % file_path)
        else:
            printerr("Error saving: %s" % new_path)
    else:
        printerr("Error loading: %s" % file_path)

func _on_files_moved(old_file, new_file):
    print("Files moved from %s to %s" % [old_file, new_file])

func _on_file_removed(file):
    print("File removed: %s" % file)

func _on_folder_moved(old_folder, new_folder):
    print("Folder moved from %s to %s" % [old_folder, new_folder])

func _on_folder_removed(folder):
    print("Folder removed: %s" % folder)

func _on_display_mode_changed():
    print("Display mode changed")
