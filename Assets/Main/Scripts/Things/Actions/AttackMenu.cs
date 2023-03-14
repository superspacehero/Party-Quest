using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : MonoBehaviour
{
    [SerializeField] private ThingDisplay upAttack, downAttack, leftAttack, rightAttack;

    public WeaponThing weapon
    {
        set
        {
            upAttack.thing = value.upAttack;
            downAttack.thing = value.downAttack;
            leftAttack.thing = value.sideAttack;
            rightAttack.thing = value.sideAttack;
        }
    }
}
