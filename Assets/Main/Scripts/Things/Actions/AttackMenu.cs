using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : Menu
{
    private CharacterThing user;

    public ThingDisplay upAttack, downAttack, leftAttack, rightAttack;

    public WeaponThing weapon
    {
        get => _weapon;
        set
        {
            _weapon = value;

            if (value == null)
            {
                upAttack.thing = null;
                downAttack.thing = null;
                leftAttack.thing = null;
                rightAttack.thing = null;
            }
            else
            {
                upAttack.thing = value.upAttackSlot;
                downAttack.thing = value.downAttackSlot;
                leftAttack.thing = value.sideAttackSlot;
                rightAttack.thing = value.sideAttackSlot;
            }

            upAttack.iconButton.onClick.AddListener(() => PickAttack(upAttack.thing));
            downAttack.iconButton.onClick.AddListener(() => PickAttack(downAttack.thing));
            leftAttack.iconButton.onClick.AddListener(() => PickAttack(leftAttack.thing));
            rightAttack.iconButton.onClick.AddListener(() => PickAttack(rightAttack.thing));

            upAttack.gameObject.SetActive(upAttack.thing != null);
            downAttack.gameObject.SetActive(downAttack.thing != null);
            leftAttack.gameObject.SetActive(leftAttack.thing != null);
            rightAttack.gameObject.SetActive(rightAttack.thing != null);
        }
    }
    private WeaponThing _weapon;

    public void SetActive(bool active, CharacterThing character, WeaponThing newWeapon = null)
    {
        user = character;

        if (newWeapon != null)
            weapon = newWeapon;

        if (active && weapon != null && user.input.isPlayer)
            Select();
    }

    public void PickAttack(Vector2 direction, AttackAction attackAction)
    {
        ThingDisplay attackDisplay = null;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
                attackDisplay = rightAttack;
            else if (direction.x < 0f)
                attackDisplay = leftAttack;
        }
        else
        {
            if (direction.y > 0f)
                attackDisplay = upAttack;
            else if (direction.y < 0f)
                attackDisplay = downAttack;
        }

        if (attackDisplay != null)
        {
            attackDisplay.Select();
        }

        // Debug.Log("Attack: " + attack);
    }

    public void PickAttack(AttackSequenceThing attackSequence, AttackAction attackAction)
    {

        ThingDisplay attackDisplay = null;

        if (attackSequence == upAttack.thing)
            attackDisplay = upAttack;
        else if (attackSequence == downAttack.thing)
            attackDisplay = downAttack;
        else if (attackSequence == leftAttack.thing)
            attackDisplay = leftAttack;
        else if (attackSequence == rightAttack.thing)
            attackDisplay = rightAttack;

        if (attackDisplay != null)
        {
            attackDisplay.Select();
        }

        if (attackAction != null)
        {
            attackAction.Use(user);
        }
    }

    public void PickAttack(GameThing thing)
    {
        Vector2 direction = Vector2.zero;

        if (thing == upAttack.thing)
            direction = Vector2.up;
        else if (thing == downAttack.thing)
            direction = Vector2.down;
        else if (thing == leftAttack.thing)
            direction = Vector2.left;
        else if (thing == rightAttack.thing)
            direction = Vector2.right;

        if (user != null)
            PickAttack(direction, user.attackAction);
    }
}
