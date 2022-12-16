using UnityEngine;

    // A class representing an instrument in the song
    [System.Serializable, CreateAssetMenu(fileName = "New Music Instrument", menuName = "Music Instrument")]
    public class MusicInstrument : ScriptableObject
    {
        // The sample to be played by the instrument
        public AudioClip sample;

        // The original key
        public MusicSequencer.Note originalKey;
    }