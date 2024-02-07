extends Node
class_name PressAnyButton

signal pressed

func _input(event):
    if event.is_pressed():
        pressed.emit()