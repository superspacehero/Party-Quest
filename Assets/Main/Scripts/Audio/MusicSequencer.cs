using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSequencer : MonoBehaviour
{
    // The JSON string containing the song information
    public string jsonSong;

    // The index of the note to loop at
    private int loopIndex;

    // The song data
    private SongData songData;

    // The list of instruments
    [SerializeField] private MusicInstrumentList instruments;

    void Start()
    {
        // Start playing the notes in the tracks
        StartCoroutine(PlayNotes());
    }

    // A class representing the data in the JSON file
    [System.Serializable]
    private class SongData
    {
        public string name;
        public float tempo;
        public int loop;
        public List<Track> tracks;
    }

    [System.Serializable]
    public class Note
    {
        public char note;
        public int octave;

        public Note(char note, int? octave)
        {
            this.note = note;
            this.octave = octave ?? 0;
        }

        public Note(int note, int octave)
        {
            // Convert the note to a hex character
            this.note = int.Parse(note.ToString(), System.Globalization.NumberStyles.HexNumber).ToString("X")[0];
            this.octave = octave;
        }
    }

    // A class representing a track in the song
    [System.Serializable]
    private class Track
    {
        // The track's instrument
        public MusicInstrument instrument;

        // The track's audio source
        [System.NonSerialized]
        public AudioSource audioSource;

        // The string containing the notes in the track
        public string noteString;

        // A dictionary storing the notes and their pitches
        [System.NonSerialized]
        public List<Note> notes;
    }

    // Parses the JSON file and initializes the song's properties
    private bool ParseJSON(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return false;

        // Read the JSON string and create a SongData object
        jsonSong = jsonString;
        SongData newSongData = JsonUtility.FromJson<SongData>(jsonString);

        // Initialize the song's properties
        songData = newSongData;

        // Create an audio source for each track
        foreach (Track track in songData.tracks)
        {
            // Initialize the dictionary
            track.notes = new List<Note>();

            // Get the instrument
            track.instrument = instruments.GetInstrument(track.instrument.name);

            // Parse the notes and calculate their pitches
            for (int i = 0; i < track.noteString.Length; i++)
            {
                // Get the current note
                Note note = new Note(track.noteString[i], null);

                // Calculate the pitch of the note if it is not a space or dash
                if (note.note != ' ' && note.note != '-')
                {
                    i++;
                    int octave = int.Parse(track.noteString[i].ToString());

                    // Add the note and its pitch to the dictionary
                    track.notes.Add(note);
                }
                else
                {
                    // Add the rest/hold as a null value in the dictionary
                    track.notes.Add(note);
                }
            }

            // Create a new audio source component
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            // Set the audio source's properties
            audioSource.clip = track.instrument.sample;
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            // Assign the audio source to the track
            track.audioSource = audioSource;
        }

        return true;
    }

    private bool ParseJSON(TextAsset jsonFile)
    {
        return ParseJSON(jsonFile.text);
    }

    private bool ParseJSON()
    {
        return ParseJSON(jsonSong);
    }

    // Plays the notes in the tracks
    private IEnumerator PlayNotes()
    {
        // Parse the JSON file and initialize the song's properties
        if (string.IsNullOrEmpty(jsonSong) && !ParseJSON())
            yield break;

        // Calculate the duration of a beat based on the song's tempo
        float beatDuration = 60.0f / songData.tempo;

        WaitForSeconds beat = new WaitForSeconds(beatDuration);

        // Keep track of the current beat
        int currentBeat = 0;

        // Keep playing the song until it reaches the end
        while (currentBeat < loopIndex)
        {
            // Play the notes in each track
            foreach (Track track in songData.tracks)
            {
                // Parse the notes and play them
                for (int i = 0; i < track.notes.Count; i++)
                {
                    // Get the current note
                    Note note = track.notes[i];

                    // Play the note if it is not a space or dash
                    if (note.note != ' ' && note.note != '-')
                    {
                        // Calculate the pitch of the note
                        float pitch = CalculatePitch(note, track.instrument.originalKey);

                        // Play the note
                        track.audioSource.pitch = pitch;
                        track.audioSource.Play();
                    }
                    else if (note.note == ' ')
                    {
                        // Stop playing the note
                        track.audioSource.Stop();
                    }

                    // Wait for the next beat
                    yield return beat;
                }
            }

            // Increment the current beat
            currentBeat++;
        }
    }

    private float CalculatePitch(Note note, Note originalKey)
    {
        // Calculate the pitch offset based on the original key
        int pitchOffset = note.octave - originalKey.octave;

        // Get the distance between the note and the original key
        int noteDistance = (note.note - originalKey.note + 12) % 12;

        // Calculate the pitch based on the pitch offset and the note distance
        float pitch = 1.0f + pitchOffset + (noteDistance / 12.0f);

        return pitch;
    }

    [Sirenix.OdinInspector.Button]
    public void Play()
    {
        StartCoroutine(PlayNotes());
    }

    #region Notes

        // Sets the note at the given index in the track to the given note and octave
        private void SetNote(int trackIndex, int noteIndex, int note, int octave)
        {
            songData.tracks[trackIndex].notes[noteIndex] = new Note(note, octave);
        }

        // Adds a new note with the given note and octave to the given track at the given index
        private void AddNote(int trackIndex, int noteIndex, int note, int octave)
        {
            songData.tracks[trackIndex].notes.Insert(noteIndex, new Note(note, octave));
        }

        // Adjusts the note at the given index in the given track up by a semitone
        private void AdjustNoteUp(int trackIndex, int noteIndex)
        {
            // Get the current note and octave
            Note currentNote = songData.tracks[trackIndex].notes[noteIndex];

            // Adjust the note up by a semitone
            int newNote = currentNote.note + 1;
            int newOctave = currentNote.octave;
            if (newNote > 12)
            {
                newNote = 1;
                newOctave++;
            }

            // Set the new note and octave
            SetNote(trackIndex, noteIndex, newNote, newOctave);
        }

        // Adjusts the note at the given index in the given track down by a semitone
        private void AdjustNoteDown(int trackIndex, int noteIndex)
        {
            // Get the current note and octave
            Note currentNote = songData.tracks[trackIndex].notes[noteIndex];

            // Adjust the note down by a semitone
            int newNote = currentNote.note - 1;
            int newOctave = currentNote.octave;
            if (newNote < 1)
            {
                newNote = 12;
                newOctave--;
            }

            // Set the new note and octave
            SetNote(trackIndex, noteIndex, newNote, newOctave);
        }

    #endregion
}