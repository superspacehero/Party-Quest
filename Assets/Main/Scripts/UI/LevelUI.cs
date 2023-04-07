using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    // UI elements
    public TextMeshProUGUI levelName, levelDescription, levelAuthorName;
    public TextList levelQuests;
    public Image levelAuthorImage;
    public Selectable levelSelectable
    {
        get
        {
            if (_levelSelectable == null)
                TryGetComponent(out _levelSelectable);

            return _levelSelectable;
        }
    }
    [SerializeField] private Selectable _levelSelectable;

    // Whether to use the global level or not
    public bool useGlobalLevel = true;

    // The current level
    public Level level
    {
        get => _level;
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

                sideQuests = levelQuests.AddNewSection(I2.Loc.LocalizationManager.GetTranslation("Quest_Optional_Singular"), I2.Loc.LocalizationManager.GetTranslation("Quest_Optional_Plural"));
                foreach (QuestGoal goal in value.sideQuests)
                    sideQuests.entries.Add(new TextList.TextSection.TextEntry() { text = goal.GetDescription() });

                levelQuests.CompileString();
            }

            General.User author = General.User.GetUserFromID(value.levelAuthorID);

            if (levelAuthorImage != null)
                levelAuthorImage.sprite  = author.GetUserProfilePicture();
            if (levelAuthorName != null)
                levelAuthorName.text = author.username;

            _level = value;
        }
    }
    private Level _level;

    // Alternatively, the string for the level
    public string levelString
    {
        set => level = Level.Deserialize(value, isPreview: true);
    }

    // The main and side quests sections in the TextList
    TextList.TextSection mainQuests, sideQuests;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (useGlobalLevel && GameManager.instance?.level.levelName != null)
            level = GameManager.instance.level;
    }

    // Update the quest status in the TextList
    public void UpdateQuests()
    {
        if (levelQuests == null || GameManager.instance?.level == null)
            return;

        foreach (QuestGoal goal in GameManager.instance.level.mainQuests)
        {
            int index = GameManager.instance.level.mainQuests.IndexOf(goal);
            if (index < mainQuests.entries.Count)
                mainQuests.entries[index].isStruckOut = goal.completed;
        }

        foreach (QuestGoal goal in GameManager.instance.level.sideQuests)
        {
            int index = GameManager.instance.level.sideQuests.IndexOf(goal);
            if (index < sideQuests.entries.Count)
                sideQuests.entries[index].isStruckOut = goal.completed;
        }

        levelQuests.CompileString();
    }
}
