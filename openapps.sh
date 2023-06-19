#!/bin/bash

first_letter="$(echo $1 | head -c1)"
unity="/home/superspacehero/Applications/Unity/2022.3.2f1/Editor/Unity"
scenepath="$(pwd)/Assets/Main/Scenes"
defaultScene="Level.unity"
github="flatpak run io.github.shiftey.Desktop"
code="code-insiders"

case "$first_letter" in
    "p")
        $unity -openfile $scenepath/$defaultScene & $code . & $github &
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
        $unity -openfile $scenepath/Menus_TitleCharacter.unity & $github & $code . &
        ;;
    *)
        $unity -openfile $scenepath/$defaultScene & $github &
        ;;
esac
