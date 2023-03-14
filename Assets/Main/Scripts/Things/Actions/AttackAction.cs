using System.Collections;
using System.Collections.Generic;
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

    private enum AttackState
    {
        None,
        PickingAttack,
        PickingTarget,
        Attacking
    }
    private AttackState attackState
    {
        get => _attackState;
        set
        {
            _attackState = value;

            switch (_attackState)
            {
                case AttackState.None:
                    CancelAction();
                    break;
                case AttackState.PickingAttack:
                    break;
                case AttackState.PickingTarget:
                    break;
                case AttackState.Attacking:
                    break;
            }

            Debug.Log("AttackState: " + _attackState);
        }
    }
    private AttackState _attackState = AttackState.PickingAttack;

    public override void Use(GameThing user)
    {
        base.Use(user);

        attackState = AttackState.PickingAttack;
    }

    public override void Move(Vector2 direction)
    {
        switch (attackState)
        {
            case AttackState.PickingAttack:
                PickAttack(direction);
                break;
            case AttackState.PickingTarget:
                PickTarget(direction);
                break;
        }
    }

    private AttackSequenceThing attack;

    private void PickAttack(Vector2 direction)
    {
        if (direction.magnitude > selectionMagnitude)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                attack = weapon.sideAttack;
            }
            else if (direction.y > 0f)
                attack = weapon.upAttack;
            else if (direction.y < 0f)
                attack = weapon.downAttack;

            Debug.Log("Attack: " + attack);
        }
    }

    private void PickTarget(Vector2 direction)
    {
        if (direction.magnitude > selectionMagnitude)
        {
            MoveAction.CheckNodeOccupied(user.transform.position + (((Vector3.right * direction.x) + (Vector3.forward * direction.y)) * attack.range), out target);
        }
    }

    public override void PrimaryAction(bool pressed)
    {
        if (pressed && attackState != AttackState.Attacking)
        {
            attackState++;
        }
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed && attackState != AttackState.Attacking)
        {
            attackState--;
        }
    }
}
