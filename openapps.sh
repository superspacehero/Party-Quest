#!/bin/bash

first_letter="$(echo $1 | head -c1)"
unity="/home/superspacehero/Applications/Unity/2021.3.16f1/Editor/Unity"
scenepath="$(pwd)/Assets/Main/Scenes"
defaultScene="SampleScene.unity"
github="flatpak run io.github.shiftey.Desktop"

case "$first_letter" in
    "p")
        $unity -openfile $scenepath/$defaultScene & code . & $github &
        ;;
    "b")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.blender.Blender & $github &
        ;;
    "a")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.audacityteam.Audacity & $github &
        ;;
    "g")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.gimp.GIMP & $github &
        ;;
    "i")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.inkscape.Inkscape & $github &
        ;;
    "c")
        $unity -openfile $scenepath/$defaultScene & clip-snap-paint & $github &
        ;;
    "m")
        $unity -openfile $scenepath/Menus.unity & $github & code . &
        ;;
    *)
        $unity -openfile $scenepath/$defaultScene & $github &
        ;;
esac