using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Character Part Thing")]
public class CharacterPartThing : GameThing
{
    // CharacterThings are a subclass of GameThings that represent characters in the game.

    // CharacterThings have all the properties of GameThings, as well as a list of stats.

    // CharacterThings can be controlled by the player, by AI, or by other characters.

    public override string thingType
    {
        get => "CharacterPart";
    }

    #region Character Part Colors

        // A material property block, used to change the colors of the Red, Green, and Blue channels of the character part's sprite.
        private MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                {
                    _materialPropertyBlock = new MaterialPropertyBlock();
                }
                return _materialPropertyBlock;
            }
        }
        private MaterialPropertyBlock _materialPropertyBlock;

        private List<Renderer> partRenderers
        {
            get
            {
                if (_partRenderers.Count <= 0)
                    if (TryGetComponent(out Renderer partRenderer))
                        _partRenderers.Add(partRenderer);

                return _partRenderers;
            }
        }
        [SerializeField] private List<Renderer> _partRenderers = new List<Renderer>();

        public Color redColor
        {
            get { return _redColor; }
            set
            {
                _redColor = value;
                
                SetColor("_RedColor", _redColor);
            }
        }

        public Color greenColor
        {
            get { return _greenColor; }
            set
            {
                _greenColor = value;
                
                SetColor("_GreenColor", _greenColor);
            }
        }

        public Color blueColor
        {
            get { return _blueColor; }
            set
            {
                _blueColor = value;
                
                SetColor("_BlueColor", _blueColor);
            }
        }

        [SerializeField, Sirenix.OdinInspector.ColorPalette]
        private Color _redColor = Color.white, _greenColor = Color.white, _blueColor = Color.white;
        
        void SetColor(string colorName, Color color)
        {
            if (partRenderers.Count <= 0)
                return;

            if (partRenderers[0] != null)
                partRenderers[0].GetPropertyBlock(materialPropertyBlock);

            foreach (Renderer partRenderer in partRenderers)
            {
                materialPropertyBlock.SetColor(colorName, color);
                partRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        void OnValidate()
        {
            SetColors();
        }

        public void SetColors()
        {
            redColor = redColor;
            greenColor = greenColor;
            blueColor = blueColor;
        }

        [Sirenix.OdinInspector.Button]
        void GetRenderers()
        {
            _partRenderers.Clear();

            foreach (Renderer partRenderer in GetComponentsInChildren<Renderer>())
            {
                _partRenderers.Add(partRenderer);
            }
        }

    #endregion
}