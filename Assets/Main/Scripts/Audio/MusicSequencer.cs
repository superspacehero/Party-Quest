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
        Play();
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
        [PropertyRange(-1, 12)] public int note;
        public int octave;

        public int[] chordNotes = new int[2] { 0, 0 };

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
                    case -1: return "Rest";
                    case 0: return "Hold";
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

        // The coroutine for fading out the track
        [System.NonSerialized]
        public Coroutine fadeOutCoroutine;

        // The track's audio source
        [System.NonSerialized]
        public AudioSource[] audioSources = new AudioSource[3];

        // The string containing the notes in the track
        public string noteString;

        // A dictionary storing the notes and their pitches
        [System.NonSerialized]
        public List<Note> notes;

        public Track()
        {
            instrument = new MusicInstrument();
            audioSources = null;
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

        // Remove the existing audio sources that are not in the tracks
        DestroyAudioSources();

        // Create or reuse an audio source for each track
        foreach (Track track in songData.tracks)
        {
            // Initialize the dictionary
            track.notes = new List<Note>();

            // Get the instrument
            track.instrument = instruments.GetInstrument(track.instrument.name);

            // Parse the notes and calculate their pitches
            ParseNotes(track);

            // Reuse or create a new audio source component
            if (track.audioSources == null)
                track.audioSources = new AudioSource[3];

            // Create a GameObject to hold the audio source components
            if (track.audioSources[0] == null)
            {
                GameObject audioSourceObject = new GameObject(track.instrument.name + " Audio Source");
                audioSourceObject.transform.SetParent(transform);
                audioSourceObject.transform.localPosition = Vector3.zero;
                audioSourceObject.transform.localRotation = Quaternion.identity;
                audioSourceObject.transform.localScale = Vector3.one;

                for (int i = 0; i < track.audioSources.Length; i++)
                {
                    AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
                    track.audioSources[i] = audioSource;

                    // Reset the audio source's properties
                    ResetAudioSource(audioSource, track.instrument);
                }
            }
        }

        return songData != null;
    }

    private void ParseNotes(Track track)
    {
        // Initialize the dictionary
        track.notes = new List<Note>();

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
                note.octave = System.Convert.ToInt32(track.noteString[i].ToString());

                i++;
                note.chordNotes[0] = System.Convert.ToInt32(track.noteString[i].ToString(), 16);

                i++;
                note.chordNotes[1] = System.Convert.ToInt32(track.noteString[i].ToString(), 16);

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
    }

    private void ResetAudioSource(AudioSource audioSource, MusicInstrument instrument)
    {
        // Set the audio source's properties
        // audioSource.clip = track.instrument.sample;  // This is set when setting up the note
        audioSource.loop = instrument.loop;
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.volume = 1f;
        audioSource.pitch = 1f;
    }

    private void DestroyAudioSources()
    {

#if UNITY_EDITOR
        // Destroy the audio sources that are not in the tracks
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Audio Source"))
                DestroyImmediate(child.gameObject);
        }
#else
        // Destroy the audio sources that are not in the tracks
        foreach (Track track in songData.tracks)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Audio Source") && !child.name.Contains(track.instrument.name))
                    Destroy(child.gameObject);
            }
        }
#endif
    }

    private bool ParseJSON(TextAsset jsonFile)
    {
        return ParseJSON(jsonFile.text);
    }

    private bool ParseJSON()
    {
        return ParseJSON(jsonSong);
    }

    [Button]
    private string GetJSON()
    {
        // foreach (Track track in songData.tracks)
        // {
        //     track.noteString = "";

        //     foreach (Note note in track.notes)
        //     {
        //         if (note.note == 0)
        //             track.noteString += "-";
        //         else if (note.note == -1)
        //             track.noteString += ".";
        //         else
        //         {
        //             track.noteString += note.note.ToString("X");
        //             track.noteString += note.octave.ToString();
        //             track.noteString += note.chordNotes[0].ToString("X");
        //             track.noteString += note.chordNotes[1].ToString("X");
        //         }
        //     }
        // }

        return JsonUtility.ToJson(songData);
    }

    private Coroutine playNotesCoroutine;

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

        // Debug.Log
        // (
        //     "Playing song: " + songData.name + "\n" +
        //     "Tempo: " + songData.tempo + "\n" +
        //     "Loop length: " + loopLength + "\n" +
        //     "Tracks: " + songData.tracks.Count + "\n\n" +
        //     "JSON: " + jsonSong
        // );

        // Calculate the duration of a beat based on the song's tempo
        float beatDuration = (60.0f / songData.tempo) / 4.0f;

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

                // Stop the previous FadeOut coroutine, if it's running
                if (track.fadeOutCoroutine != null)
                {
                    StopCoroutine(track.fadeOutCoroutine);
                    track.fadeOutCoroutine = null;
                }

                PlayNote(track, note);
            }

            // Increment the current beat
            currentBeat++;

            if (songData.loop >= 0 && currentBeat >= loopLength)
                currentBeat = songData.loop;

            // Wait for the next beat
            yield return beat;
        }
    }

    private void PlayNote(Track track, Note note)
    {
        // Get the instrument from the track
        MusicInstrument instrument = track.instrument;

        // Function to play a single note on a given audio source
        void PlaySingleNote(AudioSource audioSource, Note singleNote)
        {
            AudioClip sampleClip = instrument.GetSampleForNote(singleNote);
            int noteDifference = (singleNote.octave * 12 + singleNote.note) - (instrument.samples[0].startNote.octave * 12 + instrument.samples[0].startNote.note);

            for (int i = 0; i < instrument.samples.Count; i++)
            {
                if (instrument.samples[i].clip == sampleClip)
                {
                    noteDifference = (singleNote.octave * 12 + singleNote.note) - (instrument.samples[i].startNote.octave * 12 + instrument.samples[i].startNote.note);
                    break;
                }
            }

            float pitch = Mathf.Pow(2.0f, noteDifference / 12.0f);
            audioSource.volume = 1f;
            audioSource.clip = sampleClip;
            audioSource.pitch = pitch;
            audioSource.Play();
        }

        // Play the note if it is not a hold or rest
        if (note.note > 0)
        {
            // Play the main note
            PlaySingleNote(track.audioSources[0], note);

            // Play chord notes, if any
            for (int i = 0; i < note.chordNotes.Length; i++)
            {
                if (note.chordNotes[i] > 0)
                {
                    Note chordNote = new Note(note.chordNotes[i], note.octave);
                    PlaySingleNote(track.audioSources[i + 1], chordNote);
                }
            }
        }
        else if (note.note < 0)
        {
            // Stop playing the note
            track.fadeOutCoroutine = StartCoroutine(FadeOut(track));
        }
    }


    // This coroutine will handle the fade out effect when a note is released
    private IEnumerator FadeOut(Track track)
    {
        foreach (AudioSource audioSource in track.audioSources)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                // Debug.Log("Fading out " + audioSource.ToString(), audioSource);
                float sampleLength = audioSource.clip.length;
                float releaseTime = sampleLength * track.instrument.releaseRatio;
                float startVolume = 1f;
                float elapsedTime = 0f;
                while (elapsedTime < releaseTime)
                {
                    if (audioSource == null)
                        yield break;

                    audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / releaseTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                audioSource.Stop();
                audioSource.volume = startVolume;
            }
        }
    }

    [ButtonGroup("Media Buttons")]
    public void Play()
    {
        if (playNotesCoroutine != null)
            StopCoroutine(playNotesCoroutine);
        playNotesCoroutine = StartCoroutine(PlayNotes());
    }

    [ButtonGroup("Media Buttons")]
    public void Stop()
    {
        if (playNotesCoroutine != null)
            StopCoroutine(playNotesCoroutine);
        DestroyAudioSources();
    }

    #region Notes

    // Sets the note at the given index in the track to the given note and octave
    private void SetNote(int trackIndex, int noteIndex, int note, int octave)
    {
        songData.tracks[trackIndex].notes[noteIndex] = new Note(note, octave);

        PlayNote
        (
            songData.tracks[trackIndex],
            songData.tracks[trackIndex].notes[noteIndex]
        );
    }

    // Adds a new note with the given note and octave to the given track at the given index
    private void AddNote(int trackIndex, int noteIndex, int note, int octave)
    {
        songData.tracks[trackIndex].notes.Insert(noteIndex, new Note(note, octave));

        PlayNote
        (
            songData.tracks[trackIndex],
            songData.tracks[trackIndex].notes[noteIndex]
        );
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

    [Button]
    private void RaiseNote(int trackIndex, int noteIndex)
    {
        AdjustNote(trackIndex, noteIndex, 1);
    }

    [Button]
    private void LowerNote(int trackIndex, int noteIndex)
    {
        AdjustNote(trackIndex, noteIndex, -1);
    }

    #endregion
}