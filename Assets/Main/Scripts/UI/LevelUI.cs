using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelName, levelDescription, levelQuests, levelAuthorName;
    public Image levelAuthorImage;

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
                string quests = "";
                // foreach (Quest quest in value.quests)
                // {
                //     quests += quest.questName + "\n";
                // }
                levelQuests.text = quests;
            }

            General.User author = General.User.GetUserFromID(value.levelAuthorID);

            if (levelAuthorName != null)
                levelAuthorImage.sprite  = author.GetUserProfilePicture();
            if (levelAuthorImage != null)
                levelAuthorName.text = author.username;
        }
    }
}
