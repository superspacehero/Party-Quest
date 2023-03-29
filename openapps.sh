#!/bin/bash

first_letter="$(echo $1 | head -c1)"
unity="/home/superspacehero/Applications/Unity/2021.3.16f1/Editor/Unity"
scenepath="$(pwd)/Assets/Main/Scenes"
defaultScene="SampleScene.unity"

case "$first_letter" in
    "p")
        $unity -openfile $scenepath/$defaultScene & code . & flatpak run io.github.shiftey.Desktop &
        ;;
    "b")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.blender.Blender & flatpak run io.github.shiftey.Desktop &
        ;;
    "a")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.audacityteam.Audacity & flatpak run io.github.shiftey.Desktop &
        ;;
    "g")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.gimp.GIMP & flatpak run io.github.shiftey.Desktop &
        ;;
    "i")
        $unity -openfile $scenepath/$defaultScene & flatpak run org.inkscape.Inkscape & flatpak run io.github.shiftey.Desktop &
        ;;
    "c")
        $unity -openfile $scenepath/$defaultScene & clip-snap-paint & flatpak run io.github.shiftey.Desktop &
        ;;
    "m")
        $unity -openfile $scenepath/Menus.unity & flatpak run io.github.shiftey.Desktop &
        ;;
    *)
        $unity -openfile $scenepath/$defaultScene & flatpak run io.github.shiftey.Desktop &
        ;;
esac