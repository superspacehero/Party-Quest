using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer)), ExecuteAlways]
public class RgbSprite : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private RgbSprite parentSprite;

    [SerializeField, ReadOnly]
    public List<RgbSprite> childSprites = new List<RgbSprite>();

    public bool getParentColors = true;
    public void SetChildSpriteColors(RgbSprite child)
    {
        if (child != null)
        {
            if (!child.getParentColors)
                return;

            child.redColor = redColor;
            child.greenColor = greenColor;
            child.blueColor = blueColor;
        }
    }

    public void SetChildSpriteColors()
    {
        foreach (RgbSprite child in childSprites)
        {
            SetChildSpriteColors(child);
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (transform.parent && transform.parent.TryGetComponent(out parentSprite))
            parentSprite.childSprites.Add(this);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (parentSprite != null)
            parentSprite.childSprites.Remove(this);
    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset()
    {
        OnEnable();
        OnValidate();
    }

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    void OnValidate()
    {
        redColor = redColor;
        greenColor = greenColor;
        blueColor = blueColor;
    }

    [SerializeField]
    private Material rgbMaterial;

    private Material spriteMaterial
    {
        get
        {
            if (_spriteRenderer == null)
            {
                if (TryGetComponent(out _spriteRenderer))
                    _spriteRenderer.material = rgbMaterial;
            }

            return _spriteRenderer.material;
        }
    }
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public Color redColor
    {
        get { return _redColor; }
        set
        {
            _redColor = value;

            if (spriteMaterial)
                spriteMaterial.SetColor("_RedColor", _redColor);

            SetChildSpriteColors();
        }
    }

    public Color greenColor
    {
        get { return _greenColor; }
        set
        {
            _greenColor = value;

            if (spriteMaterial)
                spriteMaterial.SetColor("_GreenColor", _greenColor);

            SetChildSpriteColors();
        }
    }

    public Color blueColor
    {
        get { return _blueColor; }
        set
        {
            _blueColor = value;

            if (spriteMaterial)
                spriteMaterial.SetColor("_BlueColor", _blueColor);

            SetChildSpriteColors();
        }
    }


    [SerializeField, ColorPalette]
    private Color _redColor = Color.white, _greenColor = Color.white, _blueColor = Color.white;
}
