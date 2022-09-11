using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI levelName, levelDescription, levelAuthorName;
    public Image levelAuthorImage;

    public Level level
    {
        set
        {
            if (levelName != null)
                levelName.text = value.levelName;
            if (levelDescription != null)
                levelDescription.text = value.levelDescription;

            General.User author = General.User.GetUserFromID(value.levelAuthorID);

            if (levelAuthorName != null)
                levelAuthorImage.sprite  = author.GetUserProfilePicture();
            if (levelAuthorImage != null)
                levelAuthorName.text = author.username;
        }
    }
}
