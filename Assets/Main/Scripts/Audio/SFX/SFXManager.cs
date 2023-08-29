using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SFXManager>();

            return _instance;
        }
    }
    private static SFXManager _instance;

    private General.ObjectPool<AudioSource> audioSourcePool = new General.ObjectPool<AudioSource>();

    private int soundIndex = 0;

    public void PlaySFX(SFXClip sfx, Vector3 position)
    {
        PlaySFX(sfx, position, 0f);
    }
    public void PlaySFX(SFXClip sfx, Vector3 position, float addedPitch)
    {
        if (sfx.clips.Count == 0)
        {
            return;
        }

        AudioSource a = audioSourcePool.GetObjectFromPool(position);

        if (sfx.clips.Count > 1)
        {
            soundIndex = 0;

            while (soundIndex == sfx.lastSoundPlayed)
                soundIndex = Random.Range(0, sfx.clips.Count - 1);

        }
        else
            soundIndex = 0;

        sfx.lastSoundPlayed = soundIndex;

        a.clip = sfx.clips[sfx.lastSoundPlayed];
        a.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
        a.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation) + addedPitch;
        a.spatialBlend = sfx.spatialBlend;
        a.reverbZoneMix = sfx.reverbMix;
        a.minDistance = sfx.rollOffMin;
        a.maxDistance = sfx.rollOffMax;
        a.rolloffMode = sfx.rolloff;

        a.transform.position = position;

        a.loop = false;

        a.Play();
        General.DelayedFunctionSeconds(this, () => audioSourcePool.ReturnObjectToPool(a), a.clip.length);
    }

    public void PlaySFX(SFXClip sfx, AudioSource a)
    {
        a.clip = sfx.clips[Random.Range(0, sfx.clips.Count)];
        a.volume = sfx.volume + Random.Range(-sfx.volumeVariation, sfx.volumeVariation);
        a.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation, sfx.pitchVariation);
        a.spatialBlend = sfx.spatialBlend;
        a.reverbZoneMix = sfx.reverbMix;
        a.minDistance = sfx.rollOffMin;
        a.maxDistance = sfx.rollOffMax;
        a.rolloffMode = sfx.rolloff;

        a.loop = false;

        a.Play();
    }
}