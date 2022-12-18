using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class MusicSequencer : MonoBehaviour
{
    // The JSON string containing the song information
    [TextArea(10, 20)] public string jsonSong;

    // The song data
    private SongData songData = new SongData();

    // The list of instruments
    [SerializeField] private MusicInstrumentList instruments;

    // The AudioMixerGroup to which the audio sources will be assigned
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    void Start()
    {
        // Start playing the notes in the tracks
        StartCoroutine(PlayNotes());
    }

    // A class representing the data in the JSON file
    [System.Serializable]
    public class SongData
    {
        public string name;
        public float tempo;
        public int loop;
        public List<Track> tracks;

        public SongData()
        {
            name = "Placeholder";
            tempo = 120f;
            loop = 0;
            tracks = new List<Track> { new Track() };
        }
    }


    [System.Serializable]
    public class Note
    {
        [PropertyRange(1, 12)] public int note;
        public int octave;

        public Note(int note, int? octave)
        {
            this.note = note;
            this.octave = octave ?? 0;
        }

        // Use Odin Inspector to make a custom property drawer for the note, allowing GetNoteName to be shown in the inspector
        [ShowInInspector, PropertyOrder(-1)]
        public string noteLetter
        {
            get
            {
                switch (note)
                {
                    case 1: return "C";
                    case 2: return "C#";
                    case 3: return "D";
                    case 4: return "D#";
                    case 5: return "E";
                    case 6: return "F";
                    case 7: return "F#";
                    case 8: return "G";
                    case 9: return "G#";
                    case 10: return "A";
                    case 11: return "A#";
                    case 12: return "B";
                    default: return "";
                }
            }
        }
    }

    // A class representing a track in the song
    [System.Serializable]
    public class Track
    {
        // Whether or not to play the track
        public bool play = true;

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

        public Track()
        {
            instrument = new MusicInstrument();
            audioSource = null;
            noteString = "";
            notes = new List<Note>();
        }
    }


    // Parses the JSON file and initializes the song's properties
    private bool ParseJSON(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return false;

        // Read the JSON string and create a SongData object
        jsonSong = jsonString;
        songData = JsonUtility.FromJson<SongData>(jsonString);

        // Remove the existing audio sources
        DestroyAudioSources();

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
                Note note = new Note(0, null);

                // Calculate the pitch of the note if it is not a space or dash
                if (track.noteString[i] != ' ' && track.noteString[i] != '-' && track.noteString[i] != '.')
                {
                    note.note = System.Convert.ToInt32(track.noteString[i].ToString(), 16);

                    i++;
                    note.octave = int.Parse(track.noteString[i].ToString());

                    // Add the note and its pitch to the dictionary
                    track.notes.Add(note);
                }
                else
                {
                    switch (track.noteString[i])
                    {
                        case '-': note.note = 0; break;
                        case '.': note.note = -1; break;
                        case ' ': continue;
                    }

                    // Add the rest/hold as a null value in the dictionary
                    track.notes.Add(note);
                }
            }

            // Create a new audio source component
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            // Set the audio source's properties
            audioSource.clip = track.instrument.sample;
            audioSource.loop = track.instrument.loop;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = audioMixerGroup;

            // Assign the audio source to the track
            track.audioSource = audioSource;
        }

        return songData != null;
    }

    private bool ParseJSON(TextAsset jsonFile)
    {
        return ParseJSON(jsonFile.text);
    }

    private bool ParseJSON()
    {
        return ParseJSON(jsonSong);
    }

    private string GetJSON()
    {
        return JsonUtility.ToJson(songData);
    }

    // Plays the notes in the tracks
    private IEnumerator PlayNotes()
    {
        // Parse the JSON file and initialize the song's properties
        if (string.IsNullOrEmpty(jsonSong) || !ParseJSON())
        {
            songData = new SongData();
            jsonSong = GetJSON();

            Debug.LogError("Something's wrong with the JSON file!");
            yield break;
        }

        // Return early if the instruments field is empty
        if (instruments == null || instruments.instruments.Count == 0)
        {
            Debug.LogError("No instruments have been added to the MusicSequencer!");
            yield break;
        }

        // Get the longest number of beats from the tracks
        int loopLength = 0;
        foreach (Track track in songData.tracks)
        {
            if (track.notes.Count > loopLength)
                loopLength = track.notes.Count;
        }

        Debug.Log
        (
            "Playing song: " + songData.name + "\n" +
            "Tempo: " + songData.tempo + "\n" +
            "Loop length: " + loopLength + "\n" +
            "Tracks: " + songData.tracks.Count + "\n\n" +
            "JSON: " + jsonSong
        );

        // Calculate the duration of a beat based on the song's tempo
        float beatDuration = (60.0f / songData.tempo);

        WaitForSecondsRealtime beat = new WaitForSecondsRealtime(beatDuration);

        // Keep track of the current beat
        int currentBeat = 0;

        // Keep playing the song until it reaches the end
        while (currentBeat < loopLength)
        {
            // Debug.Log("Beat " + (currentBeat + 1));

            // Play the notes in each track
            foreach (Track track in songData.tracks)
            {
                // If there is a note in the track at this point, play the note
                if (!track.play || track.notes.Count <= currentBeat)
                    continue;

                // Get the current note
                Note note = track.notes[currentBeat];

                // Play the note if it is not a space or dash
                if (note.note > 0)
                {
                    // Calculate the pitch of the note, and play the note
                    track.audioSource.pitch = CalculatePitch(note, track.instrument);
                    track.audioSource.Play();
                }
                else if (note.note < 0)
                {
                    // Stop playing the note
                    track.audioSource.Stop();
                }
            }

            // Increment the current beat
            currentBeat++;

            // Wait for the next beat
            yield return beat;
        }
    }

    private float CalculatePitch(Note note, MusicInstrument instrument)
    {
        // Calculate the pitch of the note
        float pitch = Mathf.Pow(2.0f, (note.note - instrument.originalKey.note) / 12.0f);

        // Apply the octave of the note
        pitch *= Mathf.Pow(2.0f, note.octave - instrument.originalKey.octave);

        Debug.Log("Note: " + note.note + ", pitch: " + pitch);

        return pitch;
    }

    [Sirenix.OdinInspector.ButtonGroup("Media Buttons")]
    public void Play()
    {
        StopCoroutine(PlayNotes());
        StartCoroutine(PlayNotes());
    }

    [Sirenix.OdinInspector.ButtonGroup("Media Buttons")]
    public void Stop()
    {
        StopCoroutine(PlayNotes());
        DestroyAudioSources();
    }

    void DestroyAudioSources()
    {
        // Destroy the audio sources
        foreach (AudioSource audioSource in GetComponents<AudioSource>())
#if UNITY_EDITOR
            DestroyImmediate(audioSource);
#else
            Destroy(audioSource);
#endif
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

    // Adjusts the note at the given index by the amount of semitones given
    private void AdjustNote(int trackIndex, int noteIndex, int semitones)
    {
        // Get the current note and octave
        Note currentNote = songData.tracks[trackIndex].notes[noteIndex];

        // Adjust the note by the given amount of semitones
        int newNote = currentNote.note + semitones;
        int newOctave = currentNote.octave;
        if (newNote > 12)
        {
            newNote -= 12;
            newOctave++;
        }
        else if (newNote < 1)
        {
            newNote += 12;
            newOctave--;
        }

        // Set the new note and octave
        SetNote(trackIndex, noteIndex, newNote, newOctave);
    }

    private void RaiseNote(int trackIndex, int noteIndex)
    {
        AdjustNote(trackIndex, noteIndex, 1);
    }

    private void LowerNote(int trackIndex, int noteIndex)
    {
        AdjustNote(trackIndex, noteIndex, -1);
    }

    [Button]
    void TestAudioSources()
    {
        foreach (Track track in songData.tracks)
        {
            if (track.audioSource != null)
                track.audioSource.Play();
        }

        foreach (AudioSource audioSource in GetComponents<AudioSource>())
            audioSource.Play();
    }

    #endregion
}