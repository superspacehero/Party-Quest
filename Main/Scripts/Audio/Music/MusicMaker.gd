extends Menu
class_name MusicMaker

enum {BLOCK, NOTE}
var edit_state: int = BLOCK:
	set(value):
		edit_state = value
		print ("Edit State: ", edit_state)
		current_note_index = tracks[current_track_index].size() - 1

@export var music_player: MusicPlayer
@export var blocks_to_generate: int = 4
@export var block_note_range: int = 6  # Maximum allowed range of deviation from previous note
@export var block_length: int = 4
@export var starting_note = 60  # MIDI value for middle C

@export var tempo: int = 120
@export var note_length: int = 4 # 1/4 note
@export var instrument: String = "Music/Instruments/Marimba" # Path to instrument sound (temporary, until instruments are added to tracks)

var tracks: Array = []:
	set(value):
		tracks = value
		print("Tracks: ", tracks)
var current_track_index: int = 0:
	set(value):
		current_track_index = value
		print("Current Track Index: ", current_track_index)
var current_note_index: int = 0:
	set(value):
		current_note_index = value
		print("Current Note Index: ", current_note_index)

var blocks: Array = []
var rests: Array = []
var current_block_index: int = 0:
	set(value):
		current_block_index = value
		# print("Current Block Index: ", current_block_index, "\nBlock: ", blocks[current_block_index])

var note_progressions = [
	[0.0, 0.0], # Sustain
	[0.0, 1.0], # Ascend
	[1.0, 0.0], # Descend
	[0.0, 0.5, 0.75, 1.0] # Quick Ascend
]

# Called when the node enters the scene tree for the first time.
func _ready():
	super._ready()
	initialize_tracks(1)  # Initialize with one track
	generate_blocks()

func initialize_tracks(track_count: int):
	tracks.clear()
	for i in range(track_count):
		tracks.append([])

func get_last_valid_note() -> int:
	var last_note = starting_note
	if tracks[current_track_index].size() > 0:
		last_note = tracks[current_track_index][-1]
		var last_note_index = tracks[current_track_index].size() - 1

		# Find the last valid note in the track
		while last_note == null and last_note_index > 0:
			last_note_index -= 1
			last_note = tracks[current_track_index][last_note_index]

		# If the last note is still -1, set it to the starting note
		if last_note == null:
			last_note = starting_note
	return last_note

func generate_blocks():
	blocks.clear()
	rests.clear()
	for i in range(blocks_to_generate):
		# Select a random note progression
		var progression = note_progressions[randi() % note_progressions.size()].duplicate(true)
		# Choose randomly whether to negate the progression
		if randi() % 2 == 0:
			for j in range(progression.size()):
				progression[j] = -progression[j]
		blocks.append(progression)

		# Generate rests (1 for rest, 0 for no rest)
		var rest_block: Array = []
		for j in range(block_length):
			if j == 0:
				rest_block.append(false)
			else:
				rest_block.append(randi() % 4 == 0)  # 25% chance of being a rest
		rests.append(rest_block)
	preview_block()

func add_block():
	var current_block = apply_block_range_and_length(blocks[current_block_index], block_note_range, block_length, rests[current_block_index])
	for note in current_block:
		tracks[current_track_index].append(note)
	generate_blocks()

func cycle_block(direction: int):
	current_block_index = (current_block_index + direction + blocks.size()) % blocks.size()
	preview_block()

func preview_block():
	var current_block = apply_block_range_and_length(blocks[current_block_index], block_note_range, block_length, rests[current_block_index])
	print("Current Block: ", current_block)
	var abc_output = output_abc([current_block])
	if music_player.is_playing:
		music_player.stop()
	music_player.play(abc_output, false)

func apply_block_range_and_length(block: Array, note_range: int, length: int, rest_block: Array) -> Array:
	var result_block: Array = []
	var last_note = get_last_valid_note()

	var track_has_notes = tracks[current_track_index].size() == 0
	length = length if track_has_notes else length + 1

	for j in range(length):
		# Calculate the interpolation factor
		var t = float(j) / (length - 1)
		# Find the start and end points in the progression for interpolation
		var start_index = int(t * (block.size() - 1))
		var end_index = min(start_index + 1, block.size() - 1)
		# Calculate the local interpolation factor
		var local_t = (t * (block.size() - 1)) - start_index
		# Interpolate between the progression points
		var step = lerp(block[start_index], block[end_index], local_t)

		# Skip the first note if the track is empty
		if j == 0 and not track_has_notes:
			continue

		# Apply rest if specified
		if rest_block[j % rest_block.size()]:
			result_block.append(null)
			continue

		# Scale the step by note_range
		step *= note_range

		var new_note = last_note + step
		new_note = clamp(new_note, 0, 127)  # Ensure MIDI note is within valid range
		result_block.append(new_note)
		last_note = new_note
	return result_block

func change_note(direction: int):
	var current_note = starting_note
	if current_note_index >= 0 and tracks[current_track_index][current_note_index] != null:
		current_note = tracks[current_track_index][current_note_index]
	current_note += direction
	current_note = clamp(current_note, 0, 127)  # Ensure MIDI note is within valid range
	
	if current_note_index >= 0:
		tracks[current_track_index][current_note_index] = current_note
	else:
		starting_note = current_note
	preview_note()

func preview_note():
	var abc_output = output_abc([[tracks[current_track_index][current_note_index]]] if tracks[current_track_index].size() > 0 else [[starting_note]])
	if music_player.is_playing:
		music_player.stop()
	music_player.play(abc_output, false)

func midi_to_note_name(midi) -> String:
	if midi == null:
		return "z"

	var note_names = ["C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B"]
	var note = int(midi) % 12
	var note_string = note_names[note]

	var octave = int(midi / 12) - 1
	var octave_string = ""
	if octave < 4:
		while octave < 4:
			octave_string += ","
			octave += 1
	elif octave > 4:
		while octave > 4:
			octave_string += "'"
			octave -= 1

	return note_string + octave_string

func output_abc(input: Array = []) -> String:
	var abc_string = ""
	# Add header information
	abc_string = "X:1\n"
	abc_string = "Q:1/4=" + str(tempo) + "\n"
	abc_string += "L:1/" + str(note_length) + "\n"

	if input.size() == 0:
		match edit_state:
			BLOCK:
				# Append the current block to the end of the current track
				var preview_input = tracks.duplicate(true)
				for note in apply_block_range_and_length(blocks[current_block_index], block_note_range, block_length, rests[current_block_index]):
					preview_input[current_track_index].append(note)
				input = preview_input
			NOTE:
				input = tracks

	for track_index in range(input.size()):
		abc_string += "T:Track " + str(track_index + 1) + "\n"
		abc_string += "M:4/4\n"  # Example, adjust as needed
		abc_string += "K:C\n"    # Example, adjust as needed
		abc_string += "V:" + str(track_index + 1) + "\n"
		abc_string += "%%" + instrument + "\n"
		for midi in input[track_index]:
			abc_string += midi_to_note_name(midi) + " "
		abc_string += "\n"
	abc_string = abc_string.strip_edges()
	print(abc_string)
	return abc_string

func move(direction: Vector2):
	if direction.length() == 0:
		return

	if direction.x != 0:
		match edit_state:
			BLOCK:
				if direction.x > 0:
					add_block()
				elif direction.x < 0 and tracks[current_track_index].size() >= 0:
					edit_state = NOTE
					preview_note()
			NOTE:
				if direction.x < 0:
					if tracks[current_track_index].size() > 0:
						current_note_index = (int)(current_note_index + direction.x) % tracks[current_track_index].size()
				elif direction.x > 0:
					current_note_index += 1
				if current_note_index > tracks[current_track_index].size() - 1:
					edit_state = BLOCK
					preview_block()
				else:
					preview_note()

	direction.y = -direction.y
	if direction.y != 0:
		match edit_state:
			BLOCK:
				cycle_block(int(direction.y))
			NOTE:
				change_note(int(direction.y))

func right(direction: Vector2):
	if direction.length() == 0:
		return

	if direction.x != 0:
		match edit_state:
			BLOCK:
				block_length = max(block_length + int(direction.x), 1)
				preview_block()
			NOTE:
				pass

	direction.y = -direction.y
	if direction.y != 0:
		match edit_state:
			BLOCK:
				block_note_range = max(block_note_range + int(direction.y), 0)
				preview_block()
			NOTE:
				pass

func primary(pressed: bool):
	if input_ready and pressed:
		match edit_state:
			BLOCK:
				preview_block()
			NOTE:
				preview_note()

func secondary(pressed: bool):
	if input_ready and pressed:
		match edit_state:
			BLOCK:
				generate_blocks()
			NOTE:
				if tracks[current_track_index].size() > 0:
					if tracks[current_track_index][current_note_index] != null:
						tracks[current_track_index][current_note_index] = null
					else:
						tracks[current_track_index].pop_at(current_note_index)
						if current_note_index > tracks[current_track_index].size() - 1:
							current_note_index = tracks[current_track_index].size() - 1
						preview_note()

func tertiary(pressed: bool):
	if input_ready and pressed:
		var abc_output = output_abc()
		if music_player.is_playing:
			music_player.stop()
		else:
			music_player.play(abc_output, false)
