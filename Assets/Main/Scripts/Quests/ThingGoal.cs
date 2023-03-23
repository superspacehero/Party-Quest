using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class ThingGoal : QuestGoal
{
    public GameThing thing;

    protected string descriptionAction = "";

    new protected LocalizedString description
    {
        get => I2.Loc.LocalizationManager.GetTranslation($"Thing_{descriptionAction}_{(thing.properNoun ? "Proper" : "Normal")}");
    }

    public override string GetDescription()
    {
        // Use description from base class, but replace all occurrences of "thing" with the name of the thing
        return description.ToString().Replace("thing", thing.name);
    }
}
