using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class SpriteAnimationSlider : MonoBehaviour
{
    public float animationProgress
    {
        get
        {
            return _animationProgress;
        }
        set
        {
            Debug.Log("Setting animation progress to " + value);
            _animationProgress = value;
            UpdateSprite();
        }
    }
    [SerializeField, Range(0f,1f)]
    private float _animationProgress = 0f;

    [System.Serializable]
    public struct SpriteTime
    {
        public Sprite sprite;
        [Range(0f,1f)]
        public float time;
    }

    public List<SpriteTime> sprites = new List<SpriteTime>();

    private SpriteRenderer spriteRenderer
    {
        get
        {
            if (_spriteRenderer == null)
                TryGetComponent(out _spriteRenderer);
            return _spriteRenderer;
        }
    }
    private SpriteRenderer _spriteRenderer;

    private Sprite spriteToShow;

    private void UpdateSprite()
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
                spriteToShow = spriteTime.sprite;
            else
                break;
        }

        spriteRenderer.sprite = spriteToShow;
    }

    private void OnEnable()
    {
        animationProgress = animationProgress;
    }

    private void OnValidate()
    {
        animationProgress = animationProgress;
    }
}
