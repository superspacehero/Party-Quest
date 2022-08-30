using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "SFX", fileName = "New SFX Clip")]
public class SFXClip : ScriptableObject
{
    [Space]
    [Required][InlineProperty]
    public List<AudioClip> clips = new List<AudioClip>();

    [BoxGroup("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 0.2f)]
    [BoxGroup("Settings")]
    public float volumeVariation = 0.05f;
    [Range(0f, 2f)]
    [BoxGroup("Settings")]
    public float pitch = 1f;
    [Range(0f, 0.2f)]
    [BoxGroup("Settings")]
    public float pitchVariation = 0.05f;

    [Tooltip("0 = 2d, 1 = 3d")]
    [Range(0f, 1f)]
    [BoxGroup("Spatial"),LabelText("0 = 2d | 1 = 3d")]
    public float spatialBlend = 1f;
    [Range(0f, 1f)]
    [BoxGroup("Spatial")]
    public float reverbMix = 1f;
    [BoxGroup("Spatial")]
    public AudioRolloffMode rolloff = AudioRolloffMode.Linear;
    [Range(0f, 10f)]
    [BoxGroup("Spatial")]
    public float rollOffMin = 5f;
    [Range(5f, 300f)]
    [BoxGroup("Spatial")]
    public float rollOffMax = 200f;
    
    [HideInInspector]
    public int lastSoundPlayed = 0;
}