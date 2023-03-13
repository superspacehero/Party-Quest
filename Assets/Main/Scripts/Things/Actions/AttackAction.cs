using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AttackAction : ActionThing
{
    public override string thingType => "Attack";
    public override string thingSubType => "Attack";

    public WeaponThing weapon
    {
        get
        {
            if (_weapon == null)
                _weapon = GetComponentInParent<WeaponThing>(true);

            return _weapon;
        }
    }
    private WeaponThing _weapon;

    public override string thingName
    {
        get
        {
            if (string.IsNullOrEmpty(_thingName) || _thingName == "Attack")
            {
                if (weapon != null)
                    _thingName = weapon.thingName;
            }

            return _thingName;
        }
    }

    public override Sprite thingIcon
    {
        get
        {
            if (_thingIcon == null)
            {
                if (weapon != null)
                {
                    redColor = weapon.redColor;
                    greenColor = weapon.greenColor;
                    blueColor = weapon.blueColor;

                    _thingIcon = weapon.thingIcon;
                }
            }

            return _thingIcon;
        }
    }

    private bool attacking = false;

    public override void Use(GameThing user)
    {
        base.Use(user);

        Debug.Log("Attack!");
    }

    public override void Move(Vector2 direction)
    {
        // This is the base Move() function for ActionThings.
        // It does nothing, and is overridden by subclasses.
    }

    public override void PrimaryAction(bool pressed)
    {

    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed && !attacking)
            CancelAction();
    }
}
