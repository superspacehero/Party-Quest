@tool
extends Node
class_name InstrumentSampleRenamer

@export_dir var instrument_path: String = "res://Main/Art/Audio/Music/Instruments/Percussion/"

@export var rename_files: bool = false:
	set(value):
		rename_samples()

func rename_samples():
	print("Renaming samples in ", instrument_path, "...")
	var dir = DirAccess.open(instrument_path)
	if !dir:
		print("Failed to open directory:", instrument_path)
		return

	dir.list_dir_begin()
	var file = dir.get_next()
	while file != "":
		var file_path = instrument_path + file

		print("Processing file:", file_path)
		# var file_extension = file.get_extension().to_lower()
		
		var note_number = int(file.get_basename())
		var note_name = get_note_name(note_number)
		
		if note_name != "":
			var new_file_name = file_path.replace(file.get_basename(), note_name)
			var result = dir.rename(file_path, new_file_name)
			
			if result == OK:
				print("Renamed file:", file_path, " to ", new_file_name)
			else:
				print("Failed to rename file:", file_path)
		else:
			print("Failed to get note name for file:", file_path)

		file = dir.get_next()

func get_note_name(note_number: int) -> String:
	var note_names = [
		"C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B"
	]
	
	var octave = note_number / 12 - 1
	var note_index = note_number % 12
	
	return note_names[note_index] + str(octave)