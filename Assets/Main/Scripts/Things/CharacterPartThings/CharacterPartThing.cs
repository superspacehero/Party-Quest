using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game Things/Character Part Thing")]
public class CharacterPartThing : GameThing
{
    // CharacterPartThings are the parts that make up a character.

    public override string thingType
    {
        get => "CharacterPart";
    }

    protected override bool useColor
    {
        get => true;
    }

    [System.Serializable]
    public struct CharacterPartInfo
    {
        public string prefabName;
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
            prefabName = (originalPrefab) ? originalPrefab.name : name,
            redColor = redColor,
            greenColor = greenColor,
            blueColor = blueColor
        };
    }

    public static bool Instantiate(out CharacterPartThing part, CharacterPartList partList, string prefabName, Transform parent, Color redColor = default, Color greenColor = default, Color blueColor = default)
    {
        if (partList == null)
        {
            Debug.LogError("CharacterPartList is null");
            part = null;
            return false;
        }

        if (!string.IsNullOrEmpty(prefabName))
        {
            GameObject prefab = partList.characterParts.Find(prefab => prefab.name == prefabName);
            if (prefab != null && Instantiate(prefab, parent).TryGetComponent(out CharacterPartThing characterPartThing))
            {
                characterPartThing.name = prefabName;

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
                Debug.LogError("Could not instantiate " + prefabName + " as CharacterPartThing");
                part = null;
            }
        }
        else
        {
            Debug.LogError("Could not find prefab with name " + prefabName);
            part = null;
        }

        return part != null;
    }

    public static bool Instantiate(out CharacterPartThing part, CharacterPartList partList, CharacterPartInfo characterPartInfo, Transform parent)
    {
        return Instantiate(out part,
        partList,
        characterPartInfo.prefabName,
        parent, characterPartInfo.redColor,
        characterPartInfo.greenColor,
        characterPartInfo.blueColor);
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