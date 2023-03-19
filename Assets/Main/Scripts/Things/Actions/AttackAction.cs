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

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (user != null)
            transform.position = user.transform.position;
    }

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

    private AttackMenu attackMenu
    {
        get
        {
            if (_attackMenu == null)
            {
                if (user is CharacterThing)
                    _attackMenu = (user as CharacterThing).attackMenu;
            }

            return _attackMenu;
        }
    }
    private AttackMenu _attackMenu;

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

            attackMenu.SetActive(_attackState == AttackState.PickingAttack, weapon);

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

            // GameplayCamera.SetCameraObject((_attackState == AttackState.PickingAttack) ? this : user);

            // Debug.Log("AttackState: " + _attackState);
        }
    }
    private AttackState _attackState = AttackState.PickingAttack;

    public override void Use(GameThing user)
    {
        if (!actionRunning)
        {
            this.user = user;
            actionRunning = true;

            user.actionList.SetAction(this);

            gameObject.SetActive(true);
        }

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
            case AttackState.Attacking:
                if (attack != null && target != null)
                    attack.Move(direction);
                break;
        }
    }

    private AttackSequenceThing attack;

    private void PickAttack(Vector2 direction)
    {
        if (direction.magnitude > selectionMagnitude)
        {
            attackMenu.PickAttack(direction, out attack);
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
        switch (attackState)
        {
            default:
                if (pressed)
                    attackState++;
                break;
            case AttackState.PickingAttack:
                if (pressed && attack != null)
                    attackState++;
                break;
            case AttackState.PickingTarget:
                if (pressed && target != null)
                    attackState++;
                break;
            case AttackState.Attacking:
                if (attack != null && target != null)
                    attack.PrimaryAction(pressed);
                break;
        }
    }

    public override void SecondaryAction(bool pressed)
    {
        switch (attackState)
        {
            default:
                if (pressed)
                    attackState--;
                break;
            case AttackState.Attacking:
                if (attack != null && target != null)
                    attack.SecondaryAction(pressed);
                break;
        }
    }
}
