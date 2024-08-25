@tool
extends Node
class_name ABCParser

const ABC_NOTES = ["C", "D", "E", "F", "G", "A", "B"]
const ABC_ACCIDENTALS = ["^", "-", "_"]
const ABC_REST = "Z"
const HOLD_NOTE = ["-", "+"]

var note: Dictionary = {
	"A": 9, "B": 11, "C": 0, "D": 2, "E": 4, "F": 5, "G": 7
}

@export var transpose_window_theme: Theme = null # Theme for the transpose window

static var debug: bool = false

static func parse_abc(abc_string: String) -> Dictionary:
	var parsed_data = {}
	var lines = abc_string.strip_edges().split("\n")
	var current_track = ""
	var default_note_length = 1.0 / 8.0 # Default to eighth note length (if not specified)

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
			var velocity_value = 1.0 # Default to max velocity

			match token[1]:
				HOLD_NOTE[0], HOLD_NOTE[1]:
					# Handle note hold by extending the previous note
					if current_track != "" and current_track in parsed_data and parsed_data[current_track].size() > 0:
						var last_note = parsed_data[current_track][-1]
						last_note["duration"] += default_note_length

						# Store bend or velocity for later hold adjustments
						if not last_note.has("hold_bend"):
							last_note["hold_bend"] = 0 # Initialize bend if not present
						if not last_note.has("hold_velocity"):
							last_note["hold_velocity"] = velocity_value # Initialize velocity if not present

						# Check if the hold contains bend or velocity adjustments
						if token.length() > 2 and token[2] in HOLD_NOTE: # Hold contains a bend or velocity adjustment
							last_note["hold_bend"] += parse_bend(token, 2) # Incrementally apply bend during hold
							last_note["hold_velocity"] = parse_velocity(token, 0) # Apply velocity change on hold
					# Continue to the next token
					continue
				ABC_REST:
					note_pitch = "z" # Represent rest as "z"
				_:
					var i = 0

					# Handle accidental or rest
					if token[i] in ABC_ACCIDENTALS:
						if token[i] != "-": # "-" is no accidental, so skip adding it
							note_pitch += token[i]
						i += 1

					# Read the note letter
					if i < token.length() and token[i].to_upper() in ABC_NOTES:
						note_pitch += token[i].to_upper()
						i += 1

						# Read the octave number
						if i < token.length() and token[i].is_valid_int():
							note_pitch += token.substr(i)
							i += token.substr(i).length()

			if debug:
				print("Parsed note:", note_pitch) # Debugging print

			# Add the parsed note to the track
			if current_track != "" and current_track in parsed_data:
				if note_pitch != "":
					parsed_data[current_track].append({
						"pitch": note_pitch,
						"duration": default_note_length,
						"start_time": 0.0,					# Initialize start_time
						"bend": 0,							# Initialize bend
						"velocity": velocity_value,			# Initialize velocity
						"hold_bend": 0,					# Initialize hold bend
						"hold_velocity": velocity_value,	# Initialize hold velocity
					})

	# Calculate start times for each note
	for track in parsed_data.keys():
		var current_time = 0.0
		for note in parsed_data[track]:
			note["start_time"] = current_time
			current_time += note["duration"]
			if debug:
				print("Note start time calculated: ", note) # Debugging print

	return parsed_data

# Parse the bend value (e.g., -+3 or --3)
static func parse_bend(token: String, i: int) -> int:
	if not token[i].is_valid_hex_number():
		return 0 # Default to no bend

	var direction = 1 if token[i] == "+" else -1
	return direction * token[i].to_int() # E.g., -+3 -> 3, --3 -> -3

# Parse the velocity (e.g., 0-- or F--)
static func parse_velocity(token: String, i: int) -> float:
	if not token[i].is_valid_hex_number():
		return 1.0 # Default to max velocity

	var hex_val = token[i].hex_to_int()
	return hex_val / 15.0 # Normalize from 0 to 1

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
	
	# Parse the columns and get the tracker width (number of vertical channels)
	var num_channels = 0
	for row in rows:
		if row.contains("|"): # Find the first row with columns to determine the number of channels
			num_channels = row.split("|").size()
			break
	
	# Initialize a list to store the converted rows for each channel (each as a separate instrument)
	var converted_rows: Array = []
	var transposed_columns: Array = []
	for i in range(num_channels):
		converted_rows.append([])
		var transposed_column = null
		
		# Make a popup window for the user to enter the column's transposal value
		var popup: Popup = Popup.new()
		popup.set_title("Enter Transpose Value for Channel " + str(i))
		popup.borderless = false

		var vbox: VBoxContainer = VBoxContainer.new()
		
		var label: Label = Label.new()
		label.set_text("Enter the transposition value for channel " + str(i) + ":")
		vbox.add_child(label)
		
		var transposition: LineEdit = LineEdit.new()
		transposition.set_text("")
		vbox.add_child(transposition)

		var button: Button = Button.new()
		button.set_text("OK")
		button.pressed.connect(popup.hide)
		vbox.add_child(button)

		popup.add_child(vbox)
		popup.theme = transpose_window_theme
		get_tree().get_root().add_child(popup)
		popup.popup_centered()

		# Select the transposition line edit for the user to enter the value
		transposition.grab_focus()

		while popup.is_visible():
			await get_tree().process_frame

		if transposition.text == "":
			transposed_column = 0

		transposed_column = transposition.text.to_int()

		transposed_columns.append(transposed_column)

	# Process each column (channel) vertically
	for row in rows:
		if not row.contains("|"): # Skip tracker headers and empty rows
			continue

		var columns = row.split("|")
		for channel_idx in range(columns.size()):
			var col = columns[channel_idx].strip_edges()

			# Skip empty columns
			if col == "":
				continue
			
			# Check for hold, rest, or note
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
	for channel_idx in range(num_channels):
		# Start each instrument with "V:" and a newline
		output_lines.append("\nV:" + str(channel_idx) + "\n%%Music/Instruments/Piano\n")

		# Add notes with bar separators and new lines after the specified number of notes
		var note_count = 0
		for i in range(converted_rows[channel_idx].size()):
			# Ensure first note in a bar isn't a hold, convert it to a rest if necessary
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