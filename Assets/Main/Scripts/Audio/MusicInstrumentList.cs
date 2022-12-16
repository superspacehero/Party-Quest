using System.Collections.Generic;
using UnityEngine;

    // A class representing an instrument in the song
    [System.Serializable, CreateAssetMenu(fileName = "New Music Instrument List", menuName = "Music Instrument List")]
    public class MusicInstrumentList : ScriptableObject
    {
        public List<MusicInstrument> instruments;

        public MusicInstrument GetInstrument(string name)
        {
            foreach (MusicInstrument instrument in instruments)
            {
                if (instrument.name == name)
                {
                    return instrument;
                }
            }

            return null;
        }
    }