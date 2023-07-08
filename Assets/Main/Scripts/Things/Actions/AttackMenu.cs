using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : Menu
{
    public ThingDisplay upAttack, downAttack, leftAttack, rightAttack;

    public AttackSequenceThing attackSequence;

    public AttackAction attackAction
    {
        get => _attackAction;
        set
        {
            _attackAction = value;

            upAttack.thing = value.weapon.upAttack;
            downAttack.thing = value.weapon.downAttack;
            leftAttack.thing = value.weapon.sideAttack;
            rightAttack.thing = value.weapon.sideAttack;

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
    private AttackAction _attackAction;

    public void SetActive(bool active, AttackAction attack = null)
    {
        if (attack != null)
            this.attackAction = attack;

        if (active)
        {
            Select();
            transform.rotation = Quaternion.identity;
        }
        else
            Deselect();
    }

    public void PickAttack(Vector2 direction, out AttackSequenceThing attackSequence)
    {
        attackSequence = null;

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
            attackSequence = attackDisplay.thing as AttackSequenceThing;
            attackDisplay.Select();
        }

        // Debug.Log("Attack: " + attack);
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

        PickAttack(direction, out attackAction.attackSequence);
    }
}
