using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace MusicSequencer
{
    // This class represents a single note in the music sequence.
    [Serializable]
    public class Note
    {
        public int Pitch; // The pitch of the note (e.g. C, D, E, etc.)
        public int Octave; // The octave of the note (e.g. 1, 2, 3, etc.)
        public int Duration; // The duration of the note (in beats)
        public AudioClip AudioClip; // The audio clip to play for this note
    }

    // This class represents a music sequence.
    [Serializable]
    public class Sequence
    {
        public List<Note> Notes; // The list of notes in the sequence
        public int BPM; // The tempo of the music sequence in beats per minute
    }

    // This class represents the music sequencer.
    public class Sequencer : MonoBehaviour
    {
        public Sequence CurrentSequence; // The current music sequence being edited

        // This method adds a new note to the current music sequence.
        public void AddNote(Note note)
        {
            CurrentSequence.Notes.Add(note);
        }

        // This method plays the current music sequence.
        public void Play()
        {
            // Calculate the duration of a single beat in seconds
            float beatDuration = 60f / CurrentSequence.BPM;

            // Loop through all the notes in the sequence and play each one
            foreach (var note in CurrentSequence.Notes)
            {
                // Calculate the duration of the note in seconds
                float duration = beatDuration * note.Duration;

                // Play the audio clip for the note
                AudioSource.PlayClipAtPoint(note.AudioClip, Vector3.zero, 1f);

                // Wait for the duration of the note before playing the next one
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(duration));
            }
        }
    }
}
