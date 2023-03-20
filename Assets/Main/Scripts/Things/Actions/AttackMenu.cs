using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : MonoBehaviour
{
    [SerializeField] private ThingDisplay upAttack, downAttack, leftAttack, rightAttack;

    public WeaponThing weapon
    {
        get => _weapon;
        set
        {
            _weapon = value;
            upAttack.thing = value.upAttack;
            downAttack.thing = value.downAttack;
            leftAttack.thing = value.sideAttack;
            rightAttack.thing = value.sideAttack;

            upAttack.gameObject.SetActive(value.upAttack != null);
            downAttack.gameObject.SetActive(value.downAttack != null);
            leftAttack.gameObject.SetActive(value.sideAttack != null);
            rightAttack.gameObject.SetActive(value.sideAttack != null);
        }
    }
    private WeaponThing _weapon;

    public void SetActive(bool active, WeaponThing weapon = null)
    {
        if (weapon != null)
            this.weapon = weapon;

        gameObject.SetActive(active);

        if (active)
            transform.rotation = Quaternion.identity;
    }

    public void PickAttack(Vector2 direction, out AttackSequenceThing attack)
    {
        attack = null;

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
            attack = attackDisplay.thing as AttackSequenceThing;
            attackDisplay.Select();
        }

        // Debug.Log("Attack: " + attack);
    }
}
