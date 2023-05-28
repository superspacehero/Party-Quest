using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : MonoBehaviour
{
    [SerializeField] private ThingDisplay upAttack, downAttack, leftAttack, rightAttack;

    public AttackAction attack
    {
        get => _attack;
        set
        {
            _attack = value;

            upAttack.thing = value.weapon.upAttack;
            downAttack.thing = value.weapon.downAttack;
            leftAttack.thing = value.weapon.sideAttack;
            rightAttack.thing = value.weapon.sideAttack;

            upAttack.iconButton.onClick.AddListener(() => PickAttack(upAttack.thing, out value.weapon.upAttack));
            downAttack.iconButton.onClick.AddListener(() => PickAttack(downAttack.thing, out value.weapon.downAttack));
            leftAttack.iconButton.onClick.AddListener(() => PickAttack(leftAttack.thing, out value.weapon.sideAttack));
            rightAttack.iconButton.onClick.AddListener(() => PickAttack(rightAttack.thing, out value.weapon.sideAttack));

            upAttack.gameObject.SetActive(upAttack.thing != null);
            downAttack.gameObject.SetActive(downAttack.thing != null);
            leftAttack.gameObject.SetActive(leftAttack.thing != null);
            rightAttack.gameObject.SetActive(rightAttack.thing != null);
        }
    }
    private AttackAction _attack;

    public void SetActive(bool active, AttackAction attack = null)
    {
        if (attack != null)
            this.attack = attack;

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

    public void PickAttack(GameThing thing, out AttackSequenceThing attack)
    {
        attack = null;

        ThingDisplay attackDisplay = null;

        if (thing == upAttack.thing)
            attackDisplay = upAttack;
        else if (thing == downAttack.thing)
            attackDisplay = downAttack;
        else if (thing == leftAttack.thing)
            attackDisplay = leftAttack;
        else if (thing == rightAttack.thing)
            attackDisplay = rightAttack;

        if (attackDisplay != null)
        {
            attack = attackDisplay.thing as AttackSequenceThing;
            attackDisplay.Select();
        }        

        // Debug.Log("Attack: " + attack);
    }
}
