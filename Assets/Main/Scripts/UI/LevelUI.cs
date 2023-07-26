using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    // UI elements
    public TextMeshProUGUI levelName, levelDescription, levelAuthorName;

    public Image levelImage;
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

            if (levelImage != null)
                levelImage.sprite = General.StringToSprite(value.levelPreview);

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

            if (author == null)
                author = new General.User() { username = "Unknown" };

            if (levelAuthorImage != null)
                levelAuthorImage.sprite  = author.GetUserProfilePicture();
            if (levelAuthorName != null)
                levelAuthorName.text = author.username;

            _level = value;
        }
    }
    private Level _level;
    [HideInInspector] public int levelIndex;

    // Alternatively, the string for the level
    public string levelString
    {
        get => _levelString;
        set
        {
            level = Level.Deserialize(value, isPreview: true);
            _levelString = value;
        }
    }
    private string _levelString;

    // The main and side quests sections in the TextList
    TextList.TextSection mainQuests, sideQuests;

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

    public void PlayLevel()
    {
        General.LoadLevelScene(levelString, GameMode.Play, GameManager.instance.levelScene);
    }

    public void EditLevel()
    {
        General.LoadLevelScene(levelString, GameMode.Make, GameManager.instance.levelEditorScene);
    }

    public void DownloadLevel()
    {
        // TODO: Download the level from the server
    }

    public void UploadLevel()
    {
        // TODO: Upload the level to the server
    }

    public void DeleteLevel()
    {
        Level.DeleteLevel(levelIndex);
    }

    public void ReportLevel()
    {
        // TODO: Report the level to the server
    }
}
