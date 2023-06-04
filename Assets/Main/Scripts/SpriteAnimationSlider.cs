using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// [ExecuteInEditMode]
public class SpriteAnimationSlider : MonoBehaviour
{
    [Range(0f,1f)]
    public float animationProgress;
    private float _animationProgress;

    [System.Serializable]
    public struct SpriteTime
    {
        public Sprite sprite, backSprite;
        [Range(0f,1f)]
        public float time;
    }

    public List<SpriteTime> sprites = new List<SpriteTime>();

    public bool isBackSprite;

    private SpriteRenderer spriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                if (TryGetComponent(out _spriteRenderer))
                    _spriteRenderer.TryGetComponent(out spriteMask);
            return _spriteRenderer;
        }
    }
    private SpriteRenderer _spriteRenderer;

    private SpriteMask spriteMask;

    private Sprite spriteToShow;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_animationProgress != animationProgress)
        {
            _animationProgress = animationProgress;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is null");
            return;
        }
        if (sprites.Count == 0)
        {
            Debug.LogError("No sprites in list");
            return;
        }

        foreach (SpriteTime spriteTime in sprites)
        {
            if (animationProgress >= spriteTime.time)
                spriteToShow = (isBackSprite && spriteTime.backSprite != null) ? spriteTime.backSprite : spriteTime.sprite;
            else
                break;
        }

        spriteRenderer.sprite = spriteToShow;

        if (spriteMask != null)
            spriteMask.sprite = spriteToShow;
    }

    private void OnEnable()
    {
        General.DelayedFunctionFrames(this, UpdateSprite);
    }

    private void OnValidate()
    {
        UpdateSprite();
    }
}
