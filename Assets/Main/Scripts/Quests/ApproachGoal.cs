using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachGoal : ThingGoal
{
    new protected string descriptionAction = "Approach";

    public void PrintDescription()
    {
        Debug.Log(GetDescription());
    }
}
