extends Node
class_name MusicPlayer

class Sample:
    var clip: AudioStream
    var start_note: String

    func _init(clip: AudioStream, start_note: String):
        self.clip = clip
        self.start_note = start_note

@export_multiline var song: String = """
X:389
T: Jupiter (From The Planets, Opus 32)
C: Gustav Holst
Q:1/2=40
M:3/4
L:1/8
K:Eb
V:LoopStart=0
V:1
%%Music/Instruments/Tubular Bells
G,B,| C2 CE D>B, | EF E2 D2 | CD C2B,2 | G,4 G,B,|\
C2 CE D>B, | EF G2G2 | GF E2 F2 | E4 BG |
  F2 F2 EG | F2B,2 BG | F2 F2 GB | c4 cd |\
e2 d2 c2 | B2 e2 G2 | FEF2G2 | B4 GB |
c2 ce d>B | ef e2d2 | cd c2 B2 | G4 GB |\
c2 ce d>B | ef g2 g2 | gf e2 f2 | e4 BG |
F2F2 EG | F2B,2 BG | F2 F2 GB | c4 cd |\
e2d2c2 | B2 e2 G2 | FEF2G2 | B4 GB |
c2 ce d>B | efe2d2 | cdc2B2 | G4 GB |\
c2 ce d>B | ef g2 ">"g2 | ">"g">"f">"e2">"f2 | e6 |]
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
var loop: bool = true # Enable looping by default
var is_playing: bool = false # Flag to control playback
var key_signature: String = "C"

func _ready():
    if play_on_ready:
        play()

func play(song_string: String = "", loop: bool = true):
    if song_string == "":
        song_string = song
    if debug:
        print("Playing song:\n", song_string)  # Debugging print
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
        print("No valid duration found in notes")  # Debugging print
    # smallest_duration *= 0.25 # Convert to quarter notes
    return smallest_duration

func cache_samples():
    instruments.clear()

    for track in parsed_notes.keys():
        var samples: Array[Sample] = []
        var path = instruments_per_track.get(track, "Music/Instruments/Piano") # Default to "Piano" if not specified
        var directory_path = "res://Main/Art/Audio/" + path
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
                print("Loaded samples for track ", track, ": ", samples)  # Debugging print
        else:
            print("Directory not found: " + directory_path)

func get_sample_for_note(samples: Array[Sample], note: String, audio_stream_player: AudioStreamPlayer) -> AudioStream:
    if note == "z":
        return null  # Return null for rests

    var note_midi = note_to_midi(note)
    if note_midi == -1:
        print("Invalid note: ", note)  # Debugging print
        return null  # Invalid note

    if samples.size() == 0:
        return null  # No samples found

    var closest_sample = samples[0]
    var closest_diff = abs(note_midi - note_to_midi(samples[0].start_note))

    for sample in samples:
        var sample_midi = note_to_midi(sample.start_note)
        var diff = abs(note_midi - sample_midi)
        if diff < closest_diff:
            closest_diff = diff
            closest_sample = sample

    var closest_sample_midi = note_to_midi(closest_sample.start_note)
    audio_stream_player.pitch_scale = pow(2, (note_midi - closest_sample_midi) / 12.0)

    return closest_sample.clip

func get_note_from_file_name(file_name: String) -> String:
    return file_name.split(".")[0]

func _compare_samples(a: Sample, b: Sample) -> bool:
    return is_note_lower_than(a.start_note, b.start_note)

func is_note_lower_than(note1: String, note2: String) -> bool:
    var note1_midi = note_to_midi(note1)
    var note2_midi = note_to_midi(note2)
    return note1_midi < note2_midi

func note_to_midi(note: String) -> int:
    var note_base: Dictionary = {"C": 0, "D": 2, "E": 4, "F": 5, "G": 7, "A": 9, "B": 11}
    var note_value = 0
    var octave = 4 # Default to middle octave if not specified
    var accidental = 0
    var note_letter = ""
    
    var i = 0

    if note.length() == 0:
        return -1 # Invalid note

    # Check for accidental
    while i < note.length() and note[i] in ABCParser.ABC_ACCIDENTALS:
        if note[i] == "^":
            accidental += 1
        elif note[i] == "_":
            accidental -= 1
        elif note[i] == "=":
            accidental = 0
        i += 1

    # Get the note letter
    if i < note.length():
        note_letter = note[i].to_upper()
        i += 1
    else:
        return -1 # Invalid note

    # Check for octave modifiers
    while i < note.length():
        if note[i] == "'":
            octave += 1
        elif note[i] == ",":
            octave -= 1
        i += 1

    # Calculate the MIDI note value
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

func play_notes(notes_by_track, max_duration, smallest_duration, loop):
    is_playing = true
    var note_events = []
    for track in notes_by_track.keys():
        var track_notes = []
        var current_note_time = 0.0
        for note in notes_by_track[track]:
            var note_start_time = current_note_time
            var first_event = true
            while note_start_time < current_note_time + note["duration"]:
                if first_event:
                    track_notes.append({"start_time": note_start_time, "note": note, "track": track, "play": true})
                    first_event = false
                else:
                    track_notes.append({"start_time": note_start_time, "note": {"pitch": "z", "duration": smallest_duration}, "track": track, "play": false})
                note_start_time += smallest_duration
            current_note_time += note["duration"]
        note_events.append(track_notes)

    if debug:
        print("Note events generated")
    var index = 0
    var max_index = int(max_duration / smallest_duration)

    while is_playing:
        if debug:
            print("Playback loop start")
        var start_time = Time.get_ticks_msec()
        var current_events = []
        for track_notes in note_events:
            if index < track_notes.size():
                var event = track_notes[index]
                current_events.append(event)
                if event["play"]:
                    if debug:
                        print("Playing event: ", event)  # Debugging print
                    play_note_async(event["note"], event["track"])

        await get_tree().create_timer(smallest_duration * (60.0 / beats_per_minute)).timeout
        index += 1

        if not loop and index >= max_index:
            is_playing = false
            break

        if loop and index >= max_index:
            index = 0
            # Skip to the loop start time
            var loop_start_index = int(loop_start_beat * (60.0 / beats_per_minute) / smallest_duration)
            index = loop_start_index

        var end_time = Time.get_ticks_msec()
        var elapsed_time = end_time - start_time
        if debug:
            print("Elapsed time for iteration: ", elapsed_time)  # Debugging print
        if elapsed_time < smallest_duration * (60.0 / beats_per_minute) * 1000:
            await get_tree().create_timer((smallest_duration * (60.0 / beats_per_minute) * 1000 - elapsed_time) / 1000.0).timeout

    if debug:
        print("Playback ended")

func play_note_async(note: Dictionary, track: String) -> void:
    if note["pitch"] == "z":
        return

    var player = AudioStreamPlayer.new()
    add_child(player)
    var sample = get_sample_for_note(instruments[track], note["pitch"], player)
    if sample:
        player.stream = sample
        if debug:
            print("Playing note: ", note["pitch"], " with pitch scale: ", player.pitch_scale, " for duration: ", note["duration"])
        player.play()
        await get_tree().create_timer(note["duration"] * (60.0 / beats_per_minute)).timeout
        player.stop()
    else:
        print("Sample not found for note: " + note["pitch"] + " in track: " + track)
    player.queue_free()
