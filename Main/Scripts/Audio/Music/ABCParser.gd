extends Node
class_name ABCParser

const ABC_NOTES = ["C", "D", "E", "F", "G", "A", "B"]
const ABC_LOWERCASE = ["c", "d", "e", "f", "g", "a", "b"]
const ABC_ACCIDENTALS = ["^", "=", "_"]
const ABC_OCTAVE = ["'", ","]
const ABC_REST = ["z"]
const ABC_TUPLET = ["("]
const ABC_DURATION_MODIFIERS = ["/", ".", "-"]
const ABC_BROKEN_RHYTHM = [">", "<"]
const ABC_SYMBOLS_TO_SKIP = ["\"", "[", "]"]

var note: Dictionary = {
    "A": 9, "B": 11, "C": 0, "D": 2, "E": 4, "F": 5, "G": 7
}

static var debug: bool = false

static func parse_abc(abc_string: String) -> Dictionary:
    var parsed_data = {}
    var lines = abc_string.strip_edges().split("\n")
    var current_track = ""
    var default_note_length = 1.0 / 8.0  # Default to eighth note length (if not specified)
    var tuplet_duration_modifier = 1.0
    var tuplet_notes_remaining = 0

    for line in lines:
        line = line.strip_edges()
        if line == "":
            continue
        if line.begins_with("X:") or line.begins_with("T:") or line.begins_with("M:") or line.begins_with("K:") or line.begins_with("%%MIDI") or line.begins_with("%%"):
            continue
        if line.begins_with("Q:"):
            continue
        if line.begins_with("L:"):
            default_note_length = eval_length(line.substr(2).strip_edges())
            continue
        if line.begins_with("V:"):
            current_track = line.substr(2).strip_edges()
            parsed_data[current_track] = []
            continue

        var tokens = line.split("|")
        for token in tokens:
            token = token.strip_edges()
            if token == "":
                continue

            var i = 0
            while i < token.length():
                var note_duration = default_note_length * tuplet_duration_modifier
                var note_pitch = ""
                var extend_note = false

                # Handle tuplets
                if i < token.length() and token[i] in ABC_TUPLET:
                    var tuplet_str = ""
                    i += 1
                    while i < token.length() and (token[i].is_valid_float() or token[i] == ":"):
                        tuplet_str += token[i]
                        i += 1
                    
                    var tuplet_parts = tuplet_str.split(":")
                    match tuplet_parts.size():
                        1:
                            var tuplet_num = tuplet_parts[0].to_float()
                            if tuplet_num > 0:
                                tuplet_duration_modifier = 2.0 / tuplet_num
                                tuplet_notes_remaining = tuplet_num
                        2:
                            var tuplet_num = tuplet_parts[0].to_float()
                            var tuplet_denom = tuplet_parts[1].to_float()
                            if tuplet_num > 0 and tuplet_denom > 0:
                                tuplet_duration_modifier = tuplet_denom / tuplet_num
                                tuplet_notes_remaining = tuplet_num
                        3:
                            var tuplet_num = tuplet_parts[0].to_float()
                            var tuplet_denom = tuplet_parts[1].to_float()
                            var tuplet_count = tuplet_parts[2].to_float()
                            if tuplet_num > 0 and tuplet_denom > 0 and tuplet_count > 0:
                                tuplet_duration_modifier = tuplet_denom / tuplet_num
                                tuplet_notes_remaining = tuplet_count
                    continue

                if tuplet_notes_remaining > 0:
                    note_duration = default_note_length * tuplet_duration_modifier
                    tuplet_notes_remaining -= 1

                # Skip chord symbols and markers
                if i < token.length() and token[i] in ABC_SYMBOLS_TO_SKIP:
                    match token[i]:
                        "\"":
                            i += 1
                            while i < token.length() and token[i] != "\"":
                                i += 1
                            if debug:
                                print("Found chord symbol")  # Debugging print
                            i += 1
                        "[":
                            while i < token.length() and token[i] != "]":
                                i += 1
                            if debug:
                                print("Found marker")  # Debugging print
                            i += 1
                        "]":
                            i += 1
                    continue

                # Check for rests
                if i < token.length() and token[i] == "z":
                    note_pitch = "z"
                    i += 1
                else:
                    # Read the note and any accidental
                    if i < token.length() and token[i] in ABC_ACCIDENTALS:
                        note_pitch += token[i]
                        i += 1

                    # Read the note pitch
                    if i < token.length() and (token[i].to_upper() in ABC_NOTES or token[i] in ABC_LOWERCASE):
                        note_pitch += token[i].to_upper()
                        if token[i] in ABC_LOWERCASE:
                            note_pitch += "'"
                        i += 1

                        # Handle octave modifiers (commas and apostrophes)
                        while i < token.length() and (token[i] in ABC_OCTAVE):
                            note_pitch += token[i]
                            i += 1

                # Check for note length modifiers
                var duration_str = ""
                while i < token.length():
                    if token[i].is_valid_float():
                        duration_str += token[i]
                    elif token[i] == "/":
                        if duration_str == "":
                            duration_str = "1"
                        duration_str += token[i]
                    elif token[i] == ".":
                        duration_str = "3/2"
                    elif token[i] == "-":
                        extend_note = true
                        i += 1
                        break
                    else:
                        break

                    i += 1

                if duration_str == "1/":
                    duration_str = "1/2"
                if duration_str == "":
                    duration_str = "1"
                note_duration *= eval_length(duration_str)

                if debug:
                    print("Parsed note:", note_pitch, " Duration: ", note_duration, " Duration str: ", duration_str) # Debugging print

                # Add the parsed note to the track
                if current_track != "" and current_track in parsed_data:
                    if note_pitch != "":
                        if extend_note and parsed_data[current_track].size() > 0:
                            parsed_data[current_track][-1]["duration"] += note_duration
                        else:
                            parsed_data[current_track].append({
                                "pitch": note_pitch,
                                "duration": note_duration,
                                "start_time": 0.0  # Initialize start_time
                            })
                    else:
                        if debug:
                            print("Encountered empty note pitch at index ", i, " in token: ", token)  # Debugging print

                while i < token.length() and token[i] not in ABC_NOTES + ABC_LOWERCASE + ABC_ACCIDENTALS + ABC_OCTAVE + ABC_REST + ABC_DURATION_MODIFIERS:
                    i += 1

            # Reset tuplet duration modifier if no notes remaining in tuplet
            if tuplet_notes_remaining <= 0:
                tuplet_duration_modifier = 1.0

    # Calculate start times for each note
    for track in parsed_data.keys():
        var current_time = 0.0
        for note in parsed_data[track]:
            note["start_time"] = current_time
            current_time += note["duration"]
            if debug:
                print("Note start time calculated: ", note)  # Debugging print

    return parsed_data

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
