using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[ExecuteAlways]
public class RgbSprite : MonoBehaviour
{
    #region Renderers

        private enum RendererType
        {
            None,
            Sprite,
            Image,
            RawImage,
            MeshRenderer,
            SkinnedMeshRenderer
        }
        [SerializeField]
        private RendererType rendererType = RendererType.None;

        private SpriteRenderer _spriteRenderer;
        private Image _image;
        private RawImage _rawImage;
        private MeshRenderer _meshRenderer;
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        #if UNITY_EDITOR

            [SerializeField]
            private Material rgbSprite, rgbMeshVertex;

        #endif

        [Button]
        void GetRendererType()
        {
            if (TryGetComponent(out _spriteRenderer))
            {
                rendererType = RendererType.Sprite;
                #if UNITY_EDITOR
                    _spriteRenderer.material = rgbSprite;
                #endif
            }
            else if (TryGetComponent(out _image))
            {
                rendererType = RendererType.Image;
                #if UNITY_EDITOR
                    _image.material = rgbSprite;
                #endif
            }
            
            else if (TryGetComponent(out _rawImage))
            {
                rendererType = RendererType.RawImage;
                #if UNITY_EDITOR
                    _rawImage.material = rgbSprite;
                #endif
            }
            else if (TryGetComponent(out _meshRenderer))
            {
                rendererType = RendererType.MeshRenderer;
                #if UNITY_EDITOR
                    if (useVertexColors)
                        _meshRenderer.material = rgbMeshVertex;
                #endif
            }
            else if (TryGetComponent(out _skinnedMeshRenderer))
            {
                rendererType = RendererType.SkinnedMeshRenderer;
                #if UNITY_EDITOR
                    if (useVertexColors)
                        _meshRenderer.material = rgbMeshVertex;
                #endif
            }
            else
                rendererType = RendererType.None;
        }

        private bool isMeshRenderer
        {
            get => rendererType == RendererType.MeshRenderer || rendererType == RendererType.SkinnedMeshRenderer;
        }

        [SerializeField, ShowIf("isMeshRenderer")]
        private bool useVertexColors = false;

    #endregion

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
        if (rendererType == RendererType.None)
            GetRendererType();

        if (transform.parent && transform.parent.TryGetComponent(out parentSprite))
        {
            parentSprite.childSprites.Add(this);
            if (parentSprite.rendererType == rendererType && getParentColors)
                spriteMaterial = parentSprite.spriteMaterial;
        }
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
        if (spriteMaterial == null)
            return;

        redColor = redColor;
        greenColor = greenColor;
        blueColor = blueColor;
    }

    private Material spriteMaterial
    {
        get
        {
            switch (rendererType)
            {
                case RendererType.Sprite:
                    if (_spriteRenderer != null)
                        return _spriteRenderer.material;
                    break;
                case RendererType.Image:
                    if (_image != null)
                        return _image.materialForRendering;
                    break;
                case RendererType.RawImage:
                    if (_rawImage != null)
                        return _rawImage.materialForRendering;
                    break;
                case RendererType.MeshRenderer:
                    if (_meshRenderer != null)
                        return _meshRenderer.material;
                    break;
                case RendererType.SkinnedMeshRenderer:
                    if (_skinnedMeshRenderer != null)
                        return _skinnedMeshRenderer.material;
                    break;
            }

            return null;
        }

        set
        {
            switch (rendererType)
            {
                case RendererType.Sprite:
                    if (_spriteRenderer != null)
                        _spriteRenderer.material = value;
                    break;
                case RendererType.Image:
                    if (_image != null)
                        _image.material = value;
                    break;
                case RendererType.RawImage:
                    if (_rawImage != null)
                        _rawImage.material = value;
                    break;
                case RendererType.MeshRenderer:
                    if (_meshRenderer != null)
                        _meshRenderer.material = value;
                    break;
                case RendererType.SkinnedMeshRenderer:
                    if (_skinnedMeshRenderer != null)
                        _skinnedMeshRenderer.material = value;
                    break;
            }
        }
    }

    public Color redColor
    {
        get { return _redColor; }
        set
        {
            _redColor = value;

            if (spriteMaterial != null)
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

            if (spriteMaterial != null)
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

            if (spriteMaterial != null)
                spriteMaterial.SetColor("_BlueColor", _blueColor);

            SetChildSpriteColors();
        }
    }


    [SerializeField, ColorPalette]
    private Color _redColor = Color.white, _greenColor = Color.white, _blueColor = Color.white;

    // // Normal map, particularly for sprites.
    // [SerializeField]
    // private Texture2D normalMap;
}
