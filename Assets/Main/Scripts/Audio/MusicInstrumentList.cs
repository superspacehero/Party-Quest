using System.Collections.Generic;
using UnityEngine;

// A class representing an instrument in the song
[System.Serializable, CreateAssetMenu(fileName = "New Music Instrument List", menuName = "Music Instrument List")]
public class MusicInstrumentList : ScriptableObject
{
    public List<MusicInstrument> instruments = new List<MusicInstrument>();

    public MusicInstrument GetInstrument(string name)
    {
        return instruments.Find(instrument => instrument.name == name);
    }
}

[System.Serializable]
public class MusicInstrument
{
    // The name of the instrument
    public string name;

    // A list of samples for the instrument
    [System.Serializable]
    public class InstrumentSample
    {
        // The audio clip of the sample
        public AudioClip clip;

        // The starting note for this sample
        public MusicSequencer.Note startNote;
    }

    // List of samples with their start notes, in ascending order
    public List<InstrumentSample> samples = new List<InstrumentSample>();

    // The time it takes for the instrument to fade out
    [Range(0f, 1f)] public float releaseRatio = 0.5f;

    // Whether or not the instrument loops
    public bool loop;

    // Get the appropriate sample for the given note
    public AudioClip GetSampleForNote(MusicSequencer.Note note)
    {
        AudioClip selectedSample = samples[0].clip; // Default to the first sample

        for (int i = 1; i < samples.Count; i++)
        {
            // If the note is lower than the start note of the current sample, use the previous sample
            if ((note.octave < samples[i].startNote.octave) ||
                (note.octave == samples[i].startNote.octave && note.note < samples[i].startNote.note))
            {
                return selectedSample;
            }

            // Update the selected sample
            selectedSample = samples[i].clip;
        }

        // Return the last sample if the note is higher than the highest defined start note
        return selectedSample;
    }
}
