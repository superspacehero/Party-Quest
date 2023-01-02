using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[AddComponentMenu("Game Things/Game Thing")]
public class GameThing : SerializedMonoBehaviour
{
    // GameThings are a base class for all interactables and collectibles in the game.
    // They have a name, a description, an icon, and a value.
    // Their value is primarily used for determining how much they are worth when sold,
    // but can also be used for other things, such as the number a die lands on.

    // They also have a thingType, which is a string used to determine what type of thing they are,
    // and an attachedThing, which is a reference to another GameThing that is attached to this one.
    // This can be used to control an attached GameThing, or to pass information between two GameThings.

    // At their core, their primary function is to be used,
    // which is handled by the Use() function.

    // This function has a variety of uses, depending on the GameThing subclass, which include:
    // - Item GameThings on the floor, which lets them be picked up by the user;
    // - Consumable item GameThings, which leaves behind one, some, or no other GameThings on use, depending on the item;
    // - Equipment item GameThings, which can be equipped and unequipped, and which change stats on the character equipping them;
    // - Character GameThings, which, when used by another character, allow the user to take control of the target character
    //   (used for player characters, NPCs, and objects that can be directly controlled);
    // - Mechanism Trigger and Mechanism GameThings, the former of which can be used to toggle, activate, or deactivate the latter
    //   (used for doors, switches, and other objects that can be interacted with in a variety of ways);
    
    public string thingName;
    public string thingDescription;
    public Sprite thingIcon;
    public int thingValue;
    [SerializeField, FoldoutGroup("Attached Things")] protected Inventory.ThingSlot attachedThing;

    public virtual string thingType
    {
        get => "Game";
    }

    public virtual string thingSubType
    {
        get => "";
    }

    public virtual void Use(GameThing user)
    {
        // This is the base Use() function for GameThings.
        // It does nothing, and is overridden by subclasses.
    }

    public void AttachThing(GameThing thing)
    {
        attachedThing.AddThing(thing);
    }

    public void DetachThing()
    {
        attachedThing.RemoveThing();
    }

    public GameThing GetAttachedThing()
    {
        return attachedThing.thing;
    }

    #if UNITY_EDITOR

        private bool hasInventory { get { return TryGetComponent(out Inventory inventory); } }

        [Button, HideIf("hasInventory")]
        void AddInventory()
        {
            gameObject.AddComponent<Inventory>();
        }
    #endif

    [FoldoutGroup("Variables")] public GameThingVariables variables;
    
    // We need a way to be able to add variables to the GameThing class without having to modify the class itself - we can do that with a struct.
    [System.Serializable]
    public struct GameThingVariables
    {
        // A list of variables, each with a name and a value.
        public List<Variable> variables;

        // A struct to represent a single variable, with a name and a value.
        [System.Serializable]
        public struct Variable
        {
            // The name of the variable.
            [HorizontalGroup("Variable", LabelWidth = 40)] public string name;
            // The value of the variable.
            [HorizontalGroup("Variable", LabelWidth = 40)] public int value;

            // Constructor for the Variable struct.
            public Variable(string name, int value)
            {
                this.name = name;
                this.value = value;
            }
        }

        // Operator to add two instances of GameThingVariables together.
        public static GameThingVariables operator +(GameThingVariables a, GameThingVariables b)
        {
            // If a is null or b is null, return the other one.
            if (a.variables == null || b.variables == null)
                return a.variables == null ? b : a;

            // Create a new GameThingVariables instance to store the result.
            GameThingVariables result = new GameThingVariables();
            result.variables = new List<Variable>();

            // Iterate over the variables in a.
            foreach (Variable variable in a.variables)
            {
                // Check if the variable is also present in b.
                Variable otherVariable = b.variables.Find(v => v.name == variable.name);
                if (otherVariable.name != null)
                {
                    // If the variable is present in b, add the values together and add the result to the result list.
                    result.variables.Add(new Variable(variable.name, variable.value + otherVariable.value));
                }
                else
                {
                    // If the variable is not present in b, add it to the result list as-is.
                    result.variables.Add(variable);
                }
            }

            // Iterate over the variables in b.
            foreach (Variable variable in b.variables)
            {
                // Check if the variable is not present in a.
                Variable otherVariable = a.variables.Find(v => v.name == variable.name);
                if (otherVariable.name == null)
                {
                    // If the variable is not present in a, add it to the result list as-is.
                    result.variables.Add(variable);
                }
            }

            // Return the result.
            return result;
        }
    }

    #region Colors

        // A material property block, used to change the colors of the Red, Green, and Blue channels of the character part's sprite(s).
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

        private List<Renderer> renderers
        {
            get
            {
                if (_renderers.Count <= 0)
                    if (TryGetComponent(out Renderer renderer))
                        _renderers.Add(renderer);

                return _renderers;
            }
        }
        [SerializeField] private List<Renderer> _renderers = new List<Renderer>();

        public Color redColor
        {
            get { return _redColor; }
            set
            {
                _redColor = value;
                
                SetColor("_RedColor", _redColor, out _redColor);
            }
        }

        public Color greenColor
        {
            get { return _greenColor; }
            set
            {
                _greenColor = value;
                
                SetColor("_GreenColor", _greenColor, out _greenColor);
            }
        }

        public Color blueColor
        {
            get { return _blueColor; }
            set
            {
                _blueColor = value;
                
                SetColor("_BlueColor", _blueColor, out _blueColor);
            }
        }

        [SerializeField, Sirenix.OdinInspector.ColorPalette]
        private Color _redColor = Color.white, _greenColor = Color.white, _blueColor = Color.white;

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        void OnValidate()
        {
            SetColors();
        }
        
        protected void SetColor(string colorName, Color color, out Color referenceColor)
        {
            referenceColor = color;

            if (renderers.Count <= 0)
                return;

            if (renderers[0] != null)
                renderers[0].GetPropertyBlock(materialPropertyBlock);

            foreach (Renderer partRenderer in renderers)
            {
                materialPropertyBlock.SetColor(colorName, color);
                partRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        protected void SetColor(string colorName, Color color, Renderer renderer)
        {
            if (renderer != null)
                renderer.GetPropertyBlock(materialPropertyBlock);

            materialPropertyBlock.SetColor(colorName, color);
            renderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetColors()
        {
            redColor = redColor;
            greenColor = greenColor;
            blueColor = blueColor;
        }

        public void SetColors(Renderer renderer)
        {
            SetColor("_RedColor", redColor, renderer);
            SetColor("_GreenColor", greenColor, renderer);
            SetColor("_BlueColor", blueColor, renderer);
        }

        public virtual void SetColors(UnityEngine.UI.Graphic graphic)
        {
            Material materialForRendering = graphic.materialForRendering;

            materialForRendering.SetColor("_RedColor", redColor);
            materialForRendering.SetColor("_GreenColor", greenColor);
            materialForRendering.SetColor("_BlueColor", blueColor);

            graphic.material = materialForRendering;
        }


    [Sirenix.OdinInspector.Button]
        void GetRenderers()
        {
            _renderers.Clear();

            foreach (Renderer partRenderer in GetComponentsInChildren<Renderer>(includeInactive: true))
            {
                _renderers.Add(partRenderer);
            }
        }

    #endregion
}
