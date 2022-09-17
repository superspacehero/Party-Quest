using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

public class ThingGoal : Quest.QuestGoal
{
    public Thing thing;

    private string normalDescriptionKey = "Thing_Normal_", properDescriptionKey = "Thing_Proper_";
    protected string descriptionAction = "";

    new protected LocalizedString description
    {
        get => I2.Loc.LocalizationManager.GetTranslation((thing.properNoun ? properDescriptionKey : normalDescriptionKey) + descriptionAction);
    }

    public override string GetDescription()
    {
        // Use description from base class, but replace all occurrences of "thing" with the name of the thing
        return description.ToString().Replace("thing", thing.name);
    }
}
