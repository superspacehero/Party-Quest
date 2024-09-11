extends Node
class_name MusicPlayer

class Sample:
	var clip: AudioStream
	var start_note: String

	func _init(clip: AudioStream, start_note: String):
		self.clip = clip
		self.start_note = start_note

const INSTRUMENT_PATH = "res://Main/Art/Audio/"

@export var play_song: bool = false:
	set(value):
		play_song = value
		if play_song:
			play()
		elif is_playing:
			stop()

@export_multiline var song: String = """
X:1
Q:1/4=142
L:1/8
M:4/4
K:C
V:1
%%Music/Instruments/Marimba
-E4 --- --- ---|-Z- --- -C4 ---|-E4 --- -Z- ---|-G3 --- --- ---|
-F3 --- --- ---|-Z- --- --- ---|-E3 --- --- ---|-Z- --- --- ---|
-C4 --- --- ---|-E4 --- --- ---|-C4 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-D4 --- --- ---|-Z- --- -F4 ---|-F4 --- --- ---|-A3 --- --- ---|
-G3 --- --- ---|-Z- --- --- ---|-F3 --- --- ---|-Z- --- --- ---|
-D4 --- --- ---|-D4 --- --- ---|-F4 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-E4 --- --- ---|-Z- --- -E4 ---|-C4 --- -Z- ---|-E3 --- --- ---|
-F3 --- --- ---|-Z- --- --- ---|-E3 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
V:2
%%Music/Instruments/Marimba
-C4 --- --- ---|-Z- --- -E4 ---|-C4 --- -Z- ---|-E3 --- --- ---|
-A3 --- --- ---|-Z- --- --- ---|-G3 --- --- ---|-Z- --- --- ---|
-E4 --- --- ---|-C4 --- --- ---|-E4 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-F4 --- --- ---|-Z- --- -D4 ---|-D4 --- --- ---|-F3 --- --- ---|
-B3 --- --- ---|-Z- --- --- ---|-A3 --- --- ---|-Z- --- --- ---|
-F4 --- --- ---|-F4 --- --- ---|-D4 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-C4 --- --- ---|-Z- --- -C4 ---|-E4 --- -Z- ---|-G3 --- --- ---|
-A3 --- --- ---|-Z- --- --- ---|-G3 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
V:3
%%Music/Instruments/Nylon Guitar
-C3 --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-G3 --- --- ---|
-C4 --- --- ---|-Z- --- --- ---|-G3 --- --- ---|-Z- --- --- ---|
-C3 --- --- ---|-C3 --- --- ---|-C3 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-C3 --- --- ---|-Z- --- --- ---|
-D3 --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-A3 --- --- ---|
-D4 --- --- ---|-Z- --- --- ---|-A3 --- --- ---|-Z- --- --- ---|
-D3 --- --- ---|-D3 --- --- ---|-D3 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-D3 --- --- ---|-Z- --- --- ---|
-C3 --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-G3 --- --- ---|
-C4 --- --- ---|-Z- --- --- ---|-G3 --- --- ---|-Z- --- --- ---|
-C3 --- --- ---|-B2 --- --- ---|^A2 --- --- ---|-A2 --- --- ---|
^G2 --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
V:4
%%Music/Instruments/Piccolo
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-E6 --- --- ---|
-F6 --- --- ---|--- --- --- ---|-E6 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-A6 --- --- ---|
-B6 --- --- ---|--- --- --- ---|-F6 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
V:5
%%Music/Instruments/Piccolo
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-G6 --- --- ---|
-A6 --- --- ---|--- --- --- ---|-G6 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-F6 --- --- ---|
-G6 --- --- ---|--- --- --- ---|-A6 --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
V:6
%%Music/Instruments/Ethereal Horn
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-C4 --- --- ---|-B3 --- --- ---|^A3 --- --- ---|-A3 --- --- ---|
^G3 --- --- ---|--- --- --- ---|--- --- --- ---|-Z- --- --- ---|
V:7
%%Music/Instruments/Ethereal Horn
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|-Z- --- --- ---|
-E4 --- --- ---|^D4 --- --- ---|-D4 --- --- ---|^C4 --- --- ---|
-C4 --- --- ---|--- --- --- ---|--- --- --- ---|-Z- --- --- ---|
V:8
%%Music/Instruments/Percussion
^F2 --- ^F2 ---|^F2 --- ^F2 ---|^F2 --- --- ---|^F2 --- --- ---|
^F2 --- --- ---|--- --- --- ---|^F2 --- --- ---|--- --- --- ---|
^F2 --- --- ---|^F2 --- --- ---|^F2 --- --- ---|--- --- --- ---|
--- --- --- ---|--- --- --- ---|--- --- --- ---|--- --- --- ---|
^F2 --- ^F2 ---|^F2 --- ^F2 ---|^F2 --- --- ---|^F2 --- --- ---|
^F2 --- --- ---|--- --- --- ---|^F2 --- --- ---|--- --- --- ---|
^F2 --- --- ---|^F2 --- --- ---|^F2 --- --- ---|--- --- --- ---|
--- --- --- ---|--- --- --- ---|--- --- --- ---|--- --- --- ---|
^F2 --- ^F2 ---|^F2 --- ^F2 ---|^F2 --- --- ---|^F2 --- --- ---|
^F2 --- --- ---|--- --- --- ---|^F2 --- --- ---|--- --- --- ---|
--- --- --- ---|--- --- --- ---|--- --- --- ---|--- --- --- ---|
--- --- --- ---|--- --- --- ---|^D2 --- --- ---|--- --- --- ---|
"""

@export var play_on_ready: bool = true

@export var debug: bool = false:
	set(value):
		debug = value
		ABCParser.debug = value

var instruments: Dictionary = {} # Dictionary to hold instrument samples for each track
var instruments_per_track: Dictionary = {} # Dictionary to hold instrument assignments per track
var parsed_notes = {}
var beats_per_minute: float = 120.0 # Default BPM
var loop_start_beat: float = 0.0 # Default loop start beat
var looping: bool = true # Enable looping by default
var is_playing: bool = false # Flag to control playback
var key_signature: String = "C"

func _ready():
	if play_on_ready and not Engine.is_editor_hint():
		play()

func play(song_string: String = "", loop: bool = true):
	if song_string == "":
		song_string = song
	if debug:
		print("Playing song:\n", song_string) # Debugging print
	parse_song_header(song_string)
	parsed_notes = ABCParser.parse_abc(song_string)
	cache_samples()
	apply_key_signature()
	var max_duration = calculate_max_duration(parsed_notes)
	var smallest_duration = get_smallest_duration(parsed_notes)
	play_notes(parsed_notes, max_duration, smallest_duration, loop)

func stop():
	is_playing = false

func parse_song_header(song_string: String):
	var lines = song_string.strip_edges().split("\n")
	var current_track = ""
	for line in lines:
		if line.begins_with("Q:"):
			var tempo_parts = line.substr(2).strip_edges().split("=")
			if tempo_parts.size() == 2:
				beats_per_minute = tempo_parts[1].to_float()
			else:
				beats_per_minute = line.substr(2).to_float()
		elif line.begins_with("K:"):
			key_signature = line.substr(2).strip_edges()
		elif line.begins_with("V:LoopStart="):
			loop_start_beat = line.substr(12).to_float()
		elif line.begins_with("V:"):
			current_track = line.substr(2).strip_edges()
		elif line.begins_with("%%"):
			if current_track != "":
				var path = line.substr(2).strip_edges()
				instruments_per_track[current_track] = path

func calculate_max_duration(notes_by_track):
	var max_duration = 0.0
	for track in notes_by_track.keys():
		var track_duration = 0.0
		for note in notes_by_track[track]:
			track_duration += note["duration"]
		if track_duration > max_duration:
			max_duration = track_duration
	return max_duration

func get_smallest_duration(notes_by_track):
	var smallest_duration = INF
	for track in notes_by_track.keys():
		for note in notes_by_track[track]:
			if note["duration"] < smallest_duration:
				smallest_duration = note["duration"]
	if smallest_duration == float('inf'):
		smallest_duration = 1.0 # Fallback to 1 if no valid duration found

		if debug:
			push_error("No valid duration found in notes") # Debugging print
	return smallest_duration

func cache_samples():
	instruments.clear()

	for track in parsed_notes.keys():
		var samples: Array[Sample] = []
		var path = instruments_per_track.get(track, "Music/Instruments/Piano") # Default to "Piano" if not specified
		var directory_path = INSTRUMENT_PATH + path
		var dir = DirAccess.open(directory_path)
		if dir:
			dir.list_dir_begin()
			var file_name = dir.get_next()
			while file_name != "":
				if dir.current_is_dir() == false: # and not file_name.ends_with(".import"):
					file_name = file_name.replace(".import", "")
					var note = get_note_from_file_name(file_name)
					path = directory_path + "/" + file_name
					var clip = load(path)
					samples.append(Sample.new(clip, note))
				file_name = dir.get_next()
			dir.list_dir_end()
			samples.sort_custom(_compare_samples)
			instruments[track] = samples
			if debug:
				print("Loaded samples for track ", track, ": ", samples) # Debugging print
		else:
			push_error("Directory not found: " + directory_path)

func get_sample_for_note(samples: Array[Sample], note: Dictionary, stream: AudioStreamPlayer) -> AudioStream:
	if note["pitch"] in ABCParser.REST_SYMBOLS:
		return null # Return null for rests

	var note_midi = note_to_midi(note["pitch"])
	if note_midi == -1:
		push_error("Invalid note: ", note) # Debugging print
		return null # Invalid note

	if samples.size() == 0:
		return null # No samples found

	# Find the sample with the closest start note to the requested note 
	var closest_sample = get_closest_sample(samples, note["pitch"])

	var closest_sample_midi = note_to_midi(closest_sample.start_note)

	var midi = note_midi - closest_sample_midi
	stream.pitch_scale = get_pitch(midi)
	note["midi"] = midi

	return closest_sample.clip

func get_pitch(midi_note: int) -> float:
	# Calculate pitch scale based on MIDI note difference
	var pitch = pow(2, (midi_note) / 12.0)
	return pitch

func get_closest_sample(samples: Array[Sample], note: String) -> Sample:
	var note_midi = note_to_midi(note)
	if note_midi == -1:
		push_error("Invalid note: ", note) # Debugging print
		return null # Invalid note

	if samples.size() == 0:
		return null # No samples found

	# Find the sample with the closest start note to the requested note 
	var closest_sample = samples[-1]
	var closest_sample_midi = note_to_midi(closest_sample.start_note)
	var closest_sample_diff = abs(note_midi - closest_sample_midi)

	for sample in samples:
		var sample_midi = note_to_midi(sample.start_note)
		var sample_diff = abs(note_midi - sample_midi)
		if sample_diff < closest_sample_diff:
			closest_sample = sample
			closest_sample_diff = sample_diff

	return closest_sample

func get_note_from_file_name(file_name: String) -> String:
	# Remove the file extension
	var note = file_name.split(".")[0]

	# Handle accidentals
	var accidental = "-"
	if note.begins_with("^") or note.begins_with("_"):
		accidental = note[0]
		note = note.substr(1)

	# Extract the note letter
	var note_letter = note[0].to_upper()

	# Extract the octave
	var octave_string = note.substr(1)
	var octave = octave_string.to_int()

	return accidental + note_letter + str(octave)

func _compare_samples(a: Sample, b: Sample) -> bool:
	return is_note_lower_than(a.start_note, b.start_note)

func is_note_lower_than(note1: String, note2: String) -> bool:
	var note1_midi = note_to_midi(note1)
	var note2_midi = note_to_midi(note2)
	return note1_midi < note2_midi

func note_to_midi(note: String) -> int:
	var note_base: Dictionary = {"C": 0, "D": 2, "E": 4, "F": 5, "G": 7, "A": 9, "B": 11}
	var note_value = 0
	var accidental = 0
	var octave = 4 # Default octave

	# Parse accidental
	var i = 0
	if note.begins_with("^"):
		accidental += 1
		i += 1
	elif note.begins_with("_"):
		accidental -= 1
		i += 1
	elif note.begins_with("-"):
		accidental = 0
		i += 1

	# Parse note letter
	var note_letter = note[i].to_upper()
	i += 1

	# Parse octave
	var octave_string = note.substr(i)
	if octave_string.is_valid_int():
		octave = octave_string.to_int()
	
	note_value = note_base.get(note_letter, -1)
	if note_value == -1:
		return -1 # Invalid note letter

	note_value += accidental
	return (octave + 1) * 12 + note_value

func apply_key_signature():
	var key_accidentals = get_key_signature_accidentals(key_signature)
	for track in parsed_notes.keys():
		for note in parsed_notes[track]:
			if note["pitch"] in key_accidentals:
				note["pitch"] = key_accidentals[note["pitch"]]

func get_key_signature_accidentals(key: String) -> Dictionary:
	var accidentals = {}
	match key:
		"C": accidentals = {}
		"G": accidentals = {"F": "^F"}
		"D": accidentals = {"F": "^F", "C": "^C"}
		"A": accidentals = {"F": "^F", "C": "^C", "G": "^G"}
		"E": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D"}
		"B": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A"}
		"F#": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A", "E": "^E"}
		"C#": accidentals = {"F": "^F", "C": "^C", "G": "^G", "D": "^D", "A": "^A", "E": "^E", "B": "^B"}
		"F": accidentals = {"B": "_B"}
		"Bb": accidentals = {"B": "_B", "E": "_E"}
		"Eb": accidentals = {"B": "_B", "E": "_E", "A": "_A"}
		"Ab": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D"}
		"Db": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G"}
		"Gb": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G", "C": "_C"}
		"Cb": accidentals = {"B": "_B", "E": "_E", "A": "_A", "D": "_D", "G": "_G", "C": "_C", "F": "_F"}
		_: accidentals = {} # Default to no accidentals for unknown keys
	return accidentals

var last_played_notes: Dictionary = {}

func play_notes(notes_by_track: Dictionary, max_duration: float, smallest_duration: float, loop: bool):
	is_playing = true
	var streams: Array = [] # List of audio stream players per channel
	var note_time: float = smallest_duration * (60.0 / beats_per_minute)  # Time per smallest note in seconds

	# Prepare audio stream players for each track/channel
	for track in notes_by_track.keys():
		var stream = AudioStreamPlayer.new()
		add_child(stream)
		streams.append(stream)

	var index = 0
	var max_index = int(max_duration / smallest_duration)

	while is_playing:
		for track in notes_by_track.keys():
			if index >= len(notes_by_track[track]):
				continue

			var note = notes_by_track[track][index]
			var stream = streams[track.to_int() - 1]
			play_note_async(note, track, stream)

		await get_tree().create_timer(note_time, true).timeout
		index += 1

		# Handle looping
		if not loop and index >= max_index:
			is_playing = false
			break

		if loop and index >= max_index:
			index = 0
			# Skip to the looping start time
			var loop_start_index = int(loop_start_beat * (60.0 / beats_per_minute) / smallest_duration)
			index = loop_start_index

	if debug:
		print("Playback ended")

var last_played_midi: Dictionary = {} # To store the last played note's MIDI value for each track

func play_note_async(note: Dictionary, track: String, stream: AudioStreamPlayer) -> void:
	match note["pitch"]:
		# Handle rests
		ABCParser.REST_SYMBOLS[0], ABCParser.REST_SYMBOLS[1]:
			stream.stop()
			return

		# Handle hold notes
		ABCParser.HOLD_SYMBOLS[0], ABCParser.HOLD_SYMBOLS[1]:
			# Use the last played note's midi value and modify it with the bend
			if last_played_midi.has(track):
				note["midi"] = last_played_midi[track]
			note_velocity_pitch(stream, note, false)
			return
		
		_:
			# Otherwise, play a new note
			var sample = get_sample_for_note(instruments[track], note, stream)
			if sample:
				stream.stream = sample

				note_velocity_pitch(stream, note, true)

				stream.play()

				# Store the MIDI value for the track
				last_played_midi[track] = note["midi"]
			else:
				push_error("Sample not found for note: " + note["pitch"] + " in track: " + track)

func note_velocity_pitch(stream: AudioStreamPlayer, note: Dictionary, set_initial_pitch: bool):
	# Set the initial pitch using the midi value
	if set_initial_pitch and note.has("midi"):
		stream.pitch_scale = get_pitch(note["midi"])

	# Adjust the pitch using the midi and bend values
	if note.has("midi") and note.has("bend") and note["bend"] != 0:
		var target_pitch = get_pitch(note["midi"] + note["bend"])
		var tween = create_tween()
		tween.tween_property(stream, "pitch_scale", target_pitch, note["duration"])
	
	# Adjust the velocity if specified
	if note.has("velocity"):
		var target_volume_db = linear_to_db(clamp(note["velocity"], 0.01, 1.0))
		var tween = create_tween()
		tween.tween_property(stream, "volume_db", target_volume_db, note["duration"])
