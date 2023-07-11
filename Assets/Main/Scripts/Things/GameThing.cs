using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Pathfinding;

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

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        // If the GameManager's level is not null, and it doesn't have this GameThing in its list of things, add this GameThing to the list.
        if (GameManager.instance != null && !GameManager.instance.level.things.Contains(this))
        {
            GameManager.instance.level.things.Add(this);
        }

        if (variables.GetVariable("health") > 0 && variables.GetVariable("maxHealth") <= 0)
            variables.SetVariable("maxHealth", variables.GetVariable("health"));
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        // If the GameManager's level is not null, and it has this GameThing in its list of things, remove this GameThing from the list.
        if (GameManager.instance != null && GameManager.instance.level.things.Contains(this))
        {
            GameManager.instance.level.things.Remove(this);
        }
    }

    public virtual string thingName
    {
        get => _thingName;
        set => _thingName = value;
    }
    [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("thingName")] protected string _thingName;

    public bool properNoun = false;

    public string thingDescription;

    public virtual Sprite thingIcon
    {
        get
        {
            if (_thingIcon == null)
                if (TryGetComponent(out SpriteRenderer spriteRenderer))
                    _thingIcon = spriteRenderer.sprite;
                else
                    Debug.LogWarning($"GameThing {name} has no icon.");

            return _thingIcon;
        }
    }
    [SerializeField] protected Sprite _thingIcon;

    public virtual GraphNode currentNode
    {
        get
        {
            if (_currentNode == null)
            {
                _currentNode = AstarPath.active.GetNearest(transform.position).node;

                Debug.LogWarning($"GameThing {name} has no currentNode. Setting to nearest node {_currentNode}.");

                if (canOccupyCurrentNode)
                    Nodes.OccupyNode(_currentNode);
            }

            return _currentNode;
        }

        set
        {
            if (canOccupyCurrentNode)
                Nodes.UnoccupyNode(_currentNode);

            _currentNode = value;

            if (canOccupyCurrentNode)
                Nodes.OccupyNode(_currentNode);
        }
    }
    protected GraphNode _currentNode;

    public bool canOccupyCurrentNode = true;

    public void OccupyCurrentNode()
    {
        canOccupyCurrentNode = true;
        Nodes.OccupyNode(currentNode);
    }

    public virtual Vector3 position
    {
        get => currentNode != null ? (Vector3)currentNode.position : transform.position;
        set
        {
            currentNode = AstarPath.active.GetNearest(value).node;
            transform.position = value;
        }
    }

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

    // Method to convert the GameThing to a JSON string
    public string ToJson()
    {
        string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
        return jsonString;
    }

    // Method to convert a JSON string to a GameThing
    public static GameThing FromJson(string jsonString)
    {
        GameThing gameThing = JsonConvert.DeserializeObject<GameThing>(jsonString);
        return gameThing;
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

    private bool hasInventory { get => inventory != null; }

    [Button, HideIf("hasInventory")]
    protected void AddInventory()
    {
        if (inventory == null)
            _inventory = gameObject.AddComponent<Inventory>();
    }

    // Inventory for the thing
    public Inventory inventory
    {
        get
        {
            if (_inventory == null)
                TryGetComponent(out _inventory);
            return _inventory;
        }

        set
        {
            _inventory = value;
        }
    }
    private Inventory _inventory;

    // Action list for the thing
    public ActionList actionList
    {
        get
        {
            if (_actionList == null)
                _actionList = GetComponentInChildren<ActionList>();
            return _actionList;
        }
    }
    private ActionList _actionList;

    #region Interactions

    // Interaction list for the thing
    public Interactions interactionList
    {
        get
        {
            if (_interactionList == null)
            {
                _interactionList = GetComponentInChildren<Interactions>();
            }
            return _interactionList;
        }
    }
    private Interactions _interactionList;

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        interactionList?.OnCollisionEnter(other);
    }

    /// <summary>
    /// OnCollisionExit is called when this collider/rigidbody has
    /// stopped touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionExit(Collision other)
    {
        interactionList?.OnCollisionExit(other);
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        interactionList?.OnTriggerEnter(other);
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        interactionList?.OnTriggerExit(other);
    }

    #endregion

    public Transform thingTop
    {
        get
        {
            // If the thingTop is null, recursively check every GameObject with a tag of "ThingTop" to see if it is a descendant of the GameThing.
            if (_thingTop == null)
            {
                // Get all GameObjects with a tag of "ThingTop".
                GameObject[] thingTops = GameObject.FindGameObjectsWithTag("ThingTop");

                // For each GameObject with a tag of "ThingTop", recursively check up its hierarchy to see if it is a descendant of the GameThing.
                Transform thingTopRoot = null;
                foreach (GameObject thingTopObject in thingTops)
                {
                    thingTopRoot = thingTopObject.transform;
                    while (thingTopRoot != null)
                    {
                        if (thingTopRoot == transform)
                        {
                            _thingTop = thingTopObject.transform;
                            // Debug.Log($"Found thingTop {_thingTop.name} for {name}.");
                            break;
                        }
                        else
                            thingTopRoot = thingTopRoot.parent;
                    }
                }

                // If the thingTop is still null, set it to the transform of the GameThing.
                if (_thingTop == null)
                {
                    Debug.LogWarning($"No thingTop found for {name}. Setting to transform.");
                    _thingTop = transform;
                }
            }

            return _thingTop;
        }
    }
    private Transform _thingTop;

    public GameThingVariables variables;

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

        // Get the value of a variable.
        public int GetVariable(string name)
        {
            // If the variable list is null, return 0.
            if (variables == null)
                return 0;

            // Find the variable with the given name.
            Variable variable = variables.Find(v => v.name == name);
            // If the variable is not found, return 0.
            if (variable.name == null)
                return 0;

            // Return the value of the variable.
            return variable.value;
        }

        // Set the value of a variable.
        public void SetVariable(string name, int value)
        {
            // If the variable list is null, create a new one.
            if (variables == null)
                variables = new List<Variable>();

            // Find the variable with the given name.
            Variable variable = variables.Find(v => v.name == name);
            // If the variable is not found, add it to the list.
            if (variable.name == null)
                variables.Add(new Variable(name, value));
            // If the variable is found, set its value to the given value.
            else
            {
                if (name == "health")
                {
                    // Check if the new health value is greater than the max health value
                    int maxHealth = GetVariable("maxHealth");
                    if (value > maxHealth)
                    {
                        value = maxHealth;
                    }
                }

                variable.value = value;
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

        // Operators to compare two instances of GameThingVariables.
        public static bool Equals(GameThingVariables a, GameThingVariables b)
        {
            // If a is null or b is null, return false.
            if (a.variables == null || b.variables == null)
                return false;

            // Iterate over the variables in a.
            foreach (Variable variable in a.variables)
            {
                // Check if the variable is also present in b.
                Variable otherVariable = b.variables.Find(v => v.name == variable.name);
                if (otherVariable.name != null)
                {
                    // If the variable is present in b, check if the values are equal.
                    if (variable.value != otherVariable.value)
                        return false;
                }
                else
                {
                    // If the variable is not present in b, return false.
                    return false;
                }
            }

            // Iterate over the variables in b.
            foreach (Variable variable in b.variables)
            {
                // Check if the variable is not present in a.
                Variable otherVariable = a.variables.Find(v => v.name == variable.name);
                if (otherVariable.name == null)
                {
                    // If the variable is not present in a, return false.
                    return false;
                }
            }

            // Return true.
            return true;
        }
    }

    #region Colors

    protected virtual bool useColor { get; set; }

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
                _renderers.AddRange(GetComponentsInChildren<Renderer>());

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

    [SerializeField]
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
            if (partRenderer == null)
                continue;

            materialPropertyBlock.SetColor(colorName, color);
            partRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    protected void SetColor(string colorName, Color color, Renderer renderer)
    {
        if (renderer != null)
            renderer.GetPropertyBlock(materialPropertyBlock);
        else
        {
            Debug.LogWarning("Renderer is null!");
            return;
        }

        materialPropertyBlock.SetColor(colorName, color);
        renderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void SetColors()
    {
        if (!useColor)
            return;

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

    private Material _cachedMaterial;

    public virtual void SetColors(UnityEngine.UI.Graphic graphic)
    {
        // Check if we have a cached material
        if (_cachedMaterial == null)
        {
            // We don't have a cached material, create one
            _cachedMaterial = new Material(graphic.materialForRendering.shader);
        }

        // Set the color properties on the cached material
        _cachedMaterial.SetColor("_RedColor", redColor);
        _cachedMaterial.SetColor("_GreenColor", greenColor);
        _cachedMaterial.SetColor("_BlueColor", blueColor);

        // Set the graphic's material to the cached material
        graphic.material = _cachedMaterial;
    }

    [Sirenix.OdinInspector.Button, HideInPlayMode]
    void GetRenderers()
    {
        _renderers.Clear();

        foreach (Renderer partRenderer in GetComponentsInChildren<Renderer>(includeInactive: true))
        {
            _renderers.Add(partRenderer);
        }
    }

    #endregion

    #region Input

    // Movement input
    public virtual void Move(Vector2 direction)
    {
        if (GetAttachedThing() != null)
            GetAttachedThing().Move(direction);
    }

    // Primary input
    public virtual void PrimaryAction(bool pressed)
    {
        if (interaction != null && interaction.canInteract)
        {
            if (pressed)
                interaction.PrimaryAction?.Invoke();
        }
        else if (GetAttachedThing() != null)
            GetAttachedThing().PrimaryAction(pressed);
    }

    // Secondary input
    public virtual void SecondaryAction(bool pressed)
    {
        if (interaction != null && interaction.canInteract)
        {
            if (pressed)
                interaction.SecondaryAction?.Invoke();
        }
        else if (GetAttachedThing() != null)
            GetAttachedThing().SecondaryAction(pressed);
    }

    // Tertiary input
    public virtual void TertiaryAction(bool pressed)
    {
        if (GetAttachedThing() != null)
            GetAttachedThing().TertiaryAction(pressed);
    }

    public Interaction interaction;

    #endregion
}
