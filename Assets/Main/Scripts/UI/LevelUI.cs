using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelName, levelDescription, levelAuthorName;
    public TextList levelQuests;
    public Image levelAuthorImage;

    public bool useGlobalLevel = true;
    public Level level
    {
        set
        {
            if (levelName != null)
                levelName.text = value.levelName;
            if (levelDescription != null)
                levelDescription.text = value.levelDescription;

            if (levelQuests != null)
            {
                levelQuests.Clear();

                mainQuests = levelQuests.AddNewSection(I2.Loc.LocalizationManager.GetTranslation("Quest_Main_Singular"), I2.Loc.LocalizationManager.GetTranslation("Quest_Main_Plural"));
                foreach (QuestGoal goal in value.mainQuests)
                    mainQuests.entries.Add(new TextList.TextSection.TextEntry() { text = goal.GetDescription() });

                optionalQuests = levelQuests.AddNewSection(I2.Loc.LocalizationManager.GetTranslation("Quest_Optional_Singular"), I2.Loc.LocalizationManager.GetTranslation("Quest_Optional_Plural"));
                foreach (QuestGoal goal in value.sideQuests)
                    optionalQuests.entries.Add(new TextList.TextSection.TextEntry() { text = goal.GetDescription() });

                levelQuests.CompileString();
            }

            General.User author = General.User.GetUserFromID(value.levelAuthorID);

            if (levelAuthorName != null)
                levelAuthorImage.sprite  = author.GetUserProfilePicture();
            if (levelAuthorImage != null)
                levelAuthorName.text = author.username;
        }
    }

    TextList.TextSection mainQuests, optionalQuests;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (useGlobalLevel)
            level = GameManager.instance.level;
    }

    public void UpdateQuests()
    {
        if (levelQuests == null)
            return;

        foreach (QuestGoal goal in GameManager.instance.level.mainQuests)
            mainQuests.entries[GameManager.instance.level.mainQuests.IndexOf(goal)].isStruckOut = goal.completed;
            
        foreach (QuestGoal goal in GameManager.instance.level.sideQuests)
            optionalQuests.entries[GameManager.instance.level.sideQuests.IndexOf(goal)].isStruckOut = goal.completed;

        levelQuests.CompileString();
    }
}
