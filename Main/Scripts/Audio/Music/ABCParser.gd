@tool
extends Node
class_name ABCParser

const NOTES = ["C", "D", "E", "F", "G", "A", "B"]
const ACCIDENTALS = ["^", "-", "_"]
const REST_SYMBOLS = ["Z", "z"]
const HOLD_SYMBOLS = ["-", "+"]

var notes: Dictionary = {
	"A": 9, "B": 11, "C": 0, "D": 2, "E": 4, "F": 5, "G": 7
}

@export var transpose_window_theme: Theme = null # Theme for the transpose window

static var debug: bool = false

static func parse_abc(abc_string: String) -> Dictionary:
	var parsed_data = {}
	var lines = abc_string.strip_edges().split("\n")
	var current_track = ""
	var default_note_length = 1.0 / 8.0  # Default to eighth note length (if not specified)
	var last_rest = null  # To store the last rest details
	var next_hold_bend = 0  # To store the next hold's bend

	for line in lines:
		line = line.strip_edges()
		if line == "":
			continue
		if line.begins_with("X:") or line.begins_with("T:") or line.begins_with("M:") or line.begins_with("K:") or line.begins_with("%%MIDI") or line.begins_with("%%"):
			continue
		if line.begins_with("Q:"):
			var tempo_parts = line.substr(2).strip_edges().split("=")
			if tempo_parts.size() == 2:
				default_note_length = eval_length(tempo_parts[0])
			continue
		if line.begins_with("L:"):
			default_note_length = eval_length(line.substr(2).strip_edges())
			continue
		if line.begins_with("V:"):
			current_track = line.substr(2).strip_edges()
			parsed_data[current_track] = []
			continue

		line = line.replace("|", " ")
		var tokens = line.split(" ")
		for token in tokens:
			token = token.strip_edges()
			if token == "":
				continue

			var note_pitch = ""
			var velocity_value = 1.0  # Default to max velocity
			var pitch_bend = 0

			match token[1]:
				REST_SYMBOLS[0], REST_SYMBOLS[1]:
					# If it's a rest, store its properties to apply to the next note
					last_rest = {
						"velocity": parse_velocity(token, 0),
						"pitch": token[1],
						"bend": parse_bend(token)
					}

					velocity_value = last_rest["velocity"]  # Set velocity to the rest's velocity
					note_pitch = last_rest["pitch"]  # Add rest note
					pitch_bend = last_rest["bend"]  # Set pitch bend to the rest's bend
				HOLD_SYMBOLS[0], HOLD_SYMBOLS[1]:
					velocity_value = parse_velocity(token, 0)  # Parse velocity
					note_pitch = token[1]  # Add hold note
					pitch_bend = parse_bend(token) + next_hold_bend  # Set pitch bend to the hold's bend
					next_hold_bend = pitch_bend  # Store the hold's bend for the next hold
				_:
					# Handle accidental or rest
					if token[0] in ACCIDENTALS:
						if token[0] != "-":  # "-" is no accidental, so skip adding it
							note_pitch += token[0]
					# Read the note letter
					if token[1].to_upper() in NOTES:
						note_pitch += token[1].to_upper()
					# Read the octave number
					if token[2].is_valid_int():
						note_pitch += token[2]

			# Add the parsed note to the track, applying last rest's properties if present
			if current_track != "" and current_track in parsed_data:
				if note_pitch != "":
					var note_data = {
						"pitch": note_pitch,
						"duration": default_note_length,
						"bend": pitch_bend,  # Initialize bend
						"velocity": velocity_value,  # Initialize velocity
					}
					
					# Apply the last rest's properties if there was a rest
					if last_rest and note_data["pitch"] not in REST_SYMBOLS:
						if debug:
							print("Applied rest:", last_rest, " to note:", note_data) # Debug print
						note_data["velocity"] = last_rest["velocity"]
						note_data["bend"] += last_rest["bend"] * (-1 if last_rest["pitch"] == REST_SYMBOLS[0] else 1)
						next_hold_bend = note_data["bend"]  # Store the rest's bend for the next hold
						last_rest = null  # Clear after applying

					if debug:
						print("Parsed note:", note_data) # Debug print
					
					parsed_data[current_track].append(note_data)

	return parsed_data

# Parse the bend value (e.g., -+3 or --3)
static func parse_bend(token: String) -> int:
	if not token[2].is_valid_hex_number():
		return 0  # Default to no bend

	var direction = 1 if token[1] == "+" else -1
	var hex_val = token[2].hex_to_int()
	return direction * hex_val  # E.g., -+3 -> 3, --3 -> -3

# Parse the velocity (e.g., 0-- or F--)
static func parse_velocity(token: String, i: int) -> float:
	if not token[i].is_valid_hex_number():
		return 1.0  # Default to max velocity

	var hex_val = token[i].hex_to_int()
	return hex_val / 15.0  # Normalize from 0 to 1

static func eval_length(length_str: String) -> float:
	if "/" in length_str:
		var parts = length_str.split("/")
		match parts.size():
			1:
				return parts[0].to_float()
			_:
				var output = parts[0].to_float()
				for i in range(1, parts.size()):
					output /= parts[i].to_float()
				return output
	return length_str.to_float()

# Tracker conversion
@export_category("Tracker Conversion")

@export_multiline var tracker_song_to_convert: String = "":
	set(value):
		value = await convert_tracker_to_abc_string(value)
		print(value) # Debugging print
		# Save the converted ABC song to the clipboard
		DisplayServer.clipboard_set(value)

@export var notes_per_line: int = 16 # Adjustable number of notes before adding a newline
@export var notes_per_bar: int = 4 # Adjustable number of notes per bar (for adding '|')

func convert_tracker_to_abc_string(tracker_music: String) -> String:
	var rows = tracker_music.strip_edges().split("\n")
	var song_name = "Converted Tracker Song"
	var tempo = 120
	var note_length = "1/8"
	var key_signature = "C"
	
	# Parse the columns and get the tracker width (number of vertical channels)
	var num_channels = 0
	for row in rows:
		if row.contains("|"): # Find the first row with columns to determine the number of channels
			num_channels = row.split("|").size()
			break

	# Pop up a window to get the tempo value
	var popup: Popup = Popup.new()
	popup.set_title("Enter Tempo Value")
	popup.borderless = false

	var vbox: VBoxContainer = VBoxContainer.new()

	var label: Label = Label.new()
	label.set_text("Enter the tempo value (e.g., 120):")
	vbox.add_child(label)

	var value: LineEdit = LineEdit.new()
	value.set_text("")
	vbox.add_child(value)

	var button: Button = Button.new()
	button.set_text("OK")
	button.pressed.connect(popup.hide)
	vbox.add_child(button)

	popup.add_child(vbox)
	popup.theme = transpose_window_theme
	get_tree().get_root().add_child(popup)
	popup.popup_centered()

	# Select the tempo line edit for the user to enter the value
	value.grab_focus()
	while popup.is_visible():
		await get_tree().process_frame
	tempo = value.text.to_int()
	if tempo == 0:
		tempo = 120
	
	# Make a popup window for the user to enter the note length
	popup.set_title("Enter Note Length")
	label.set_text("Enter the note length (e.g., 1/8):")
	value.set_text("")
	popup.popup_centered()
	value.grab_focus()
	while popup.is_visible():
		await get_tree().process_frame
	note_length = value.text
	if note_length == "":
		note_length = "1/8"

	# Make a popup window for the user to enter the key signature
	popup.set_title("Enter Key Signature")
	label.set_text("Enter the key signature (e.g., C):")
	value.set_text("")
	popup.popup_centered()
	value.grab_focus()
	while popup.is_visible():
		await get_tree().process_frame
	key_signature = value.text
	if key_signature == "":
		key_signature = "C"
	
	# Initialize a list to store the converted rows for each channel (each as a separate instrument)
	var converted_rows: Array = []
	var transposed_columns: Array = []
	for i in range(num_channels):
		converted_rows.append([])
		var transposed_column = null
		
		# Make a popup window for the user to enter the column's transposal value
		popup.set_title("Enter Transpose Value for Channel " + str(i))
		label.set_text("Enter the transposition value for channel " + str(i) + ":")
		value.set_text("")
		popup.popup_centered()

		# Select the transposition line edit for the user to enter the value
		value.grab_focus()

		while popup.is_visible():
			await get_tree().process_frame

		if value.text == "":
			transposed_column = -1
		else:
			transposed_column = value.text.to_int() - 1

		transposed_columns.append(transposed_column)

	# Process each column (channel) vertically
	for row in rows:
		if row.begins_with("Name:"):
			song_name = row.split("Name: ")[1].strip_edges()

		if not row.contains("|"): # Skip tracker headers and empty rows
			continue

		var columns = row.split("|")
		for channel_idx in range(1, columns.size()):
			var col = columns[channel_idx].strip_edges()

			# Skip empty columns
			if col == "":
				continue
			
			# Check for hold, rest, or notes
			if col.begins_with("==="): # Rest
				if channel_idx < converted_rows.size():
					converted_rows[channel_idx].append("-Z-")
			elif col.begins_with("."): # Hold
				if channel_idx < converted_rows.size():
					converted_rows[channel_idx].append("---")
			else: # Note
				var note = convert_tracker_note_to_abc(col, transposed_columns[channel_idx])
				if channel_idx < converted_rows.size():
					converted_rows[channel_idx].append(note)

	# Convert the vertically collected data into separate instrument lines
	var output_lines = []
	output_lines.append("X:1\nT:" + song_name + "\nQ:" + note_length + "=" + str(tempo) + "\nK:" + key_signature + "\n") # ABC header

	for channel_idx in range(num_channels):
		# Start each instrument with "V:" and a newline
		output_lines.append("\nV:" + str(channel_idx) + "\n%%Music/Instruments/Piano\n")

		# Add notes with bar separators and new lines after the specified number of notes
		var note_count = 0
		for i in range(converted_rows[channel_idx].size()):
			# Ensure first notes in a bar isn't a hold, convert it to a rest if necessary
			if note_count % notes_per_bar == 0:
				if converted_rows[channel_idx][i] == "---":
					converted_rows[channel_idx][i] = "-Z-"

			output_lines.append(converted_rows[channel_idx][i])
			note_count += 1

			# Add space separator between notes, unless it's a bar separator
			if note_count % notes_per_bar == 0:
				output_lines.append("|") # Bar separator after 'notes_per_bar' notes
			else:
				output_lines.append(" ") # Space between regular notes

			# Add newline after every 'notes_per_line' notes
			if note_count % notes_per_line == 0:
				output_lines.append("\n")

	return "".join(output_lines)

func convert_tracker_note_to_abc(note: String, transpose: int = 0) -> String:
	var output_note = ""

	if note.length() >= 2:
		var accidental = note[1] # Note accidental
		if accidental == "#":
			output_note += "^" # Sharp
		elif accidental == "b":
			output_note += "_" # Flat
		else:
			output_note += "-" # Natural

		output_note += note[0].to_upper() # Note letter
		# Octave number
		if note[2].is_valid_int():
			output_note += String.num_int64(note[2].to_int() + transpose)
		else:
			output_note += note[2]

	return output_note