using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class AttackAction : ActionThing
{
    public override string thingType => "Attack";
    public override string thingSubType => "Attack";

    [SerializeField, Range(0f, 1f)]
    private float selectionMagnitude = 0.99f;

    private CharacterThing target;

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
        if (!attacking)
        {
            if (direction.magnitude > selectionMagnitude)
            {
                MoveAction.CheckNodeOccupied(user.transform.position + (((Vector3.right * direction.x) + (Vector3.forward * direction.y)) * weapon.range), out target);
            }
        }        
    }

    public override void PrimaryAction(bool pressed)
    {
        if (pressed && !attacking && target != null)
        {
            attacking = true;
            user.actionList.SetAction(this);

            Debug.Log("Attacking " + target.thingName);
        }
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed && !attacking)
            CancelAction();
    }
}
