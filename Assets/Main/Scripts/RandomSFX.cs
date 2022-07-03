using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Random Sound Effect", menuName = "Random Sound Effect", order = 1)]
public class RandomSFX : ScriptableObject
{
    public List<AudioClip> sounds;

    private int lastSoundPlayed;

    public AudioClip RandomSound()
    {
        if (sounds.Count > 1)
        {
            int soundIndex = 0;

            while (soundIndex == lastSoundPlayed)
                soundIndex = Mathf.RoundToInt(Random.Range(0f, sounds.Count - 1));
            
            lastSoundPlayed = soundIndex;
        }
        
        return sounds[lastSoundPlayed];
    }
}
