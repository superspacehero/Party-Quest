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

    private List<AudioSource> audioSources = new List<AudioSource>();

    private int soundIndex = 0;

    private const int INITIAL_AUDIO_SOURCES = 100;//default pool size. Pool auto expands one at a time.

    //todo: spread the frame tracker over a couple frames to reduces *DO_DO_DO_DO_DO_DOOOOOOOOOSHHHHHH*
    private Dictionary<SFXClip,int> frameTracker;//used to limit how many times an sfx plays per frame. this reduces big *DOOOSHHHHHH* from hapening
    private const int MAX_SOUND_PER_FRAME = 5;

    private void Awake()
    {
        _instance = this;

        for (int i = 0; i < INITIAL_AUDIO_SOURCES; i++)
        {
            audioSources.Add(AddAudioSource());
        }

        frameTracker = new Dictionary<SFXClip, int>();
    }

    private void LateUpdate()
    {
        foreach(SFXClip key in frameTracker.Keys.ToList())
        {
            frameTracker[key] = 0;
        }
    }

    private AudioSource AddAudioSource()
    {
        GameObject g = new GameObject();
        g.name = "Managed Audio Source";
        g.transform.parent = gameObject.transform;
        return g.AddComponent<AudioSource>();
        
    }

    public void PlaySFX(SFXClip sfx, Vector3 position)
    {
        PlaySFX(sfx,position,0f);
    }
    public void PlaySFX(SFXClip sfx, Vector3 position ,float addedPitch)
    {
        if (sfx.clips.Count == 0)
        {
            return;
        }

        if (frameTracker.ContainsKey(sfx))
        {
            if (frameTracker[sfx] >= MAX_SOUND_PER_FRAME)
            {
                return;
            }
            frameTracker[sfx]++;
        }
        else
        {
            frameTracker.Add(sfx,1);
        }

        AudioSource a;
        if (audioSources.Count < 1)
        {
            a = AddAudioSource();
        }
        else
        {
            a = audioSources[0];
            audioSources.RemoveAt(0);
        }

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
        a.volume = sfx.volume + Random.Range(-sfx.volumeVariation,sfx.volumeVariation);
        a.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation,sfx.pitchVariation) + addedPitch;
        a.spatialBlend = sfx.spatialBlend;
        a.reverbZoneMix = sfx.reverbMix;
        a.minDistance = sfx.rollOffMin;
        a.maxDistance = sfx.rollOffMax;
        a.rolloffMode = sfx.rolloff;

        a.transform.position = position;

        a.loop = false;//just in case?

        //replace with new save manager:
        //a.volume = a.volume * SaveManager.instance.GetSaveFile().settings[SettingType.SoundEffectVolume];

        a.Play();
        StartCoroutine(ReturnAudioSource(a,a.clip.length));
    }

    public void PlaySFX(SFXClip sfx ,AudioSource a)
    {
        a.clip = sfx.clips[Random.Range(0,sfx.clips.Count)];
        a.volume = sfx.volume + Random.Range(-sfx.volumeVariation,sfx.volumeVariation);
        a.pitch = sfx.pitch + Random.Range(-sfx.pitchVariation,sfx.pitchVariation);
        a.spatialBlend = sfx.spatialBlend;
        a.reverbZoneMix = sfx.reverbMix;
        a.minDistance = sfx.rollOffMin;
        a.maxDistance = sfx.rollOffMax;
        a.rolloffMode = sfx.rolloff;

        a.loop = false;//just in case?

        //replace with new save manager:
        //a.volume = a.volume * SaveManager.instance.GetSaveFile().settings[SettingType.SoundEffectVolume];

        a.Play();
    }

    IEnumerator ReturnAudioSource(AudioSource a, float t)
    {
        yield return new WaitForSecondsRealtime(t);

        audioSources.Add(a);
    }
}