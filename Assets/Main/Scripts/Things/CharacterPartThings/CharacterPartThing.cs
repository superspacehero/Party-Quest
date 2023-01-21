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