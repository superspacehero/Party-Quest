#!/bin/bash

first_letter="$(echo $1 | head -c1)"
godot="/home/superspacehero/.local/share/godot/app_userdata/Godots/versions/Godot-latest.x86_64 \"--path\" "
project_path=". -e"
github="github open ."

case "$first_letter" in
    "p")
        $godot $project_path & code . & $github &
        ;;
    "b")
        $godot $project_path & flatpak run org.blender.Blender & $github &
        ;;
    "a")
        $godot $project_path & flatpak run org.gimp.GIMP & $github &
        ;;
    "c")
        $godot $project_path & clip-snap-paint & $github &
        ;;
    *)
        $godot $project_path & $github &
        ;;
esac
