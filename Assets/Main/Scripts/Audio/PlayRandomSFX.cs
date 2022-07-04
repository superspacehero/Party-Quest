using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSFX : MonoBehaviour
{
    [SerializeField] private RandomSFX randomSFX;
    private AudioSource audioSource
    {
        get
        {
            if (_audioSource == null)
                if (TryGetComponent(out _audioSource))
                    _audioSource.outputAudioMixerGroup = randomSFX.mixerGroup;
                
            return _audioSource;
        }
    }
    private AudioSource _audioSource;

    [Button]
    public void PlaySound()
    {
        audioSource.PlayOneShot(randomSFX.RandomSound());
    }

    public void StopSounds()
    {
        audioSource.Stop();
    }

    public void ChangeSFX(RandomSFX newSFX)
    {
        randomSFX = newSFX;
    }
}
