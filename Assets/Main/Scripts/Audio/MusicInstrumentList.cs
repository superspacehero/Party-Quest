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

    // The sample to be played by the instrument
    public AudioClip sample;

    // The original key
    public MusicSequencer.Note originalKey;

    // Whether or not the instrument loops
    public bool loop;
}