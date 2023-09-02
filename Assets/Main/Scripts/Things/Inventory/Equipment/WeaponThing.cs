using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponThing : EquipmentThing
{
    public override string thingType
    {
        get => "Weapon";
    }

    [Sirenix.OdinInspector.FoldoutGroup("Attacks")]
    public AttackSequenceThing upAttackSlot, sideAttackSlot, downAttackSlot;

    protected override void Start()
    {
        base.Start();

        upAttackSlot = (upAttackSlot != null) ? Instantiate(upAttackSlot, transform) : null;
        sideAttackSlot = (sideAttackSlot != null) ? Instantiate(sideAttackSlot, transform) : null;
        downAttackSlot = (downAttackSlot != null) ? Instantiate(downAttackSlot, transform) : null;

        // Debug.Log($"Weapon {thingName} has {upAttackSlot} up, {sideAttackSlot} side, and {downAttackSlot} down attacks");
    }

    public override void Use(GameThing user)
    {
        if (user is CharacterThing)
        {
            CharacterThing character = user as CharacterThing;

            Debug.Log($"Using {thingName} as weapon for {character.thingName}");

            if (character.attackMenu != null)
            {
                character.attackMenu.SetActive(true, character, this);
            }
        }
        else
        {
            Debug.Log($"{user.thingName} is a {user.GetType()} and cannot use {thingName} as a weapon", user);
        }
    }
}