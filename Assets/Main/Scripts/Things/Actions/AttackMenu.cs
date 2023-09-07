using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : Menu
{
    private CharacterThing user;

    public ThingDisplay upAttack, middleAttack, downAttack;

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
                middleAttack.thing = null;
            }
            else
            {
                upAttack.SetThing(value.upAttackSlot, user);
                downAttack.SetThing(value.downAttackSlot, user);
                middleAttack.SetThing(value.sideAttackSlot, user);
            }

            upAttack.iconButton.onClick.AddListener(() => PickAttack(upAttack.thing as AttackSequenceThing, user.attackAction));
            downAttack.iconButton.onClick.AddListener(() => PickAttack(downAttack.thing as AttackSequenceThing, user.attackAction));
            middleAttack.iconButton.onClick.AddListener(() => PickAttack(middleAttack.thing as AttackSequenceThing, user.attackAction));

            upAttack.gameObject.SetActive(upAttack.thing != null);
            downAttack.gameObject.SetActive(downAttack.thing != null);
            middleAttack.gameObject.SetActive(middleAttack.thing != null);
        }
    }
    private WeaponThing _weapon;

    public void SetActive(bool active, CharacterThing character, WeaponThing newWeapon = null)
    {
        user = character;

        if (newWeapon != null)
            weapon = newWeapon;

        if (active && weapon != null && user.input.isPlayer)
        {
            if (middleAttack.thing != null)
                objectToSelect = middleAttack.iconButton.gameObject;
            else if (upAttack.thing != null)
                objectToSelect = upAttack.iconButton.gameObject;
            else if (downAttack.thing != null)
                objectToSelect = downAttack.iconButton.gameObject;

            Select();
        }
    }

    public void PickAttack(AttackSequenceThing attackSequence, AttackAction attackAction)
    {
        if (attackAction != null)
        {
            attackAction.Use(user, attackSequence);
        }
    }
}
