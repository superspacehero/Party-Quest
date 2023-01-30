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

    [System.Serializable]
    public struct CharacterPartInfo
    {
        public GameObject prefab;
        public Color redColor;
        public Color greenColor;
        public Color blueColor;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
    public CharacterPartInfo characterPartInfo
    {
        get => new CharacterPartInfo()
        {
            prefab = originalPrefab,
            redColor = redColor,
            greenColor = greenColor,
            blueColor = blueColor
        };
    }

    public static bool Instantiate(out CharacterPartThing part, GameObject prefab, Transform parent, Color redColor = default, Color greenColor = default, Color blueColor = default)
    {
        if (Instantiate(prefab, parent).TryGetComponent(out CharacterPartThing characterPartThing))
        {
            characterPartThing.name = prefab.name;

            characterPartThing.originalPrefab = prefab;

            if (redColor != default)
                characterPartThing.redColor = redColor;
            if (greenColor != default)
                characterPartThing.greenColor = greenColor;
            if (blueColor != default)
                characterPartThing.blueColor = blueColor;

            part = characterPartThing;
        }
        else
        {
            Debug.LogError("Could not instantiate " + prefab.name + " as CharacterPartThing");
            part = null;
        }

        return part != null;
    }

    public static bool Instantiate(out CharacterPartThing part, CharacterPartInfo characterPartInfo, Transform parent)
    {
        return Instantiate(out part, characterPartInfo.prefab, parent, characterPartInfo.redColor, characterPartInfo.greenColor, characterPartInfo.blueColor);
    }

    [HideInInspector] public GameObject originalPrefab;

    private SpriteAnimationSlider spriteAnimationSlider
    {
        get
        {
            if (_spriteAnimationSlider == null)
                TryGetComponent(out _spriteAnimationSlider);
            return _spriteAnimationSlider;
        }
    }
    private SpriteAnimationSlider _spriteAnimationSlider;

    public bool isBackPart
    {
        get => _isBackPart;
        set
        {
            if (spriteAnimationSlider != null)
            {
                spriteAnimationSlider.isBackSprite = value;
                spriteAnimationSlider.UpdateSprite();
            }

            // if (value)
            //     Debug.Log("Set " + name + " to back part");

            _isBackPart = value;
        }
    }
    private bool _isBackPart;

    public void CheckIsBackPart()
    {
        if (isBackPart)
            return;

        // Check for the first CharacterPartThing in the parent hierarchy
        CharacterPartThing parentPart = null;

        Transform parent = transform.parent;
        while (parent != null)
        {
            parentPart = parent.GetComponent<CharacterPartThing>();
            if (parentPart != null)
                break;
            parent = parent.parent;
        }

        if (parentPart != null)
        {
            isBackPart = parentPart.isBackPart;
        }
    }
}