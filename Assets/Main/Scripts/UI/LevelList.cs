using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelList : MonoBehaviour
{
    [SerializeField] private Image _levelPreviewImage;
    [SerializeField] private TextMeshProUGUI _levelNameText, _levelDescriptionText;

    private List<Level> _levels = new List<Level>();
    private Level _currentLevel
    {
        get => _levels[currentLevelIndex];
        set => _levels[currentLevelIndex] = value;
    }

    private int currentLevelIndex
    {
        get => _currentLevelIndex;
        set
        {
            _currentLevelIndex = value;
            UpdateLevelInfo();
        }
    }
    private int _currentLevelIndex = 0;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        LoadLevels();
    }

    private void LoadLevels()
    {
        _levels.Clear();

        List<Level> levelList = JsonUtility.FromJson<List<Level>>(PlayerPrefs.GetString("Levels"));

        if (levelList == null)
            return;

        foreach (Level level in levelList)
            _levels.Add(level);

        currentLevelIndex = 0;
    }

    public void SelectLevel(int index)
    {
        currentLevelIndex = index;
    }

    private void UpdateLevelInfo()
    {
        // _levelPreviewImage.sprite = _currentLevel.preview;
        _levelNameText.text = _currentLevel.levelName;
        _levelDescriptionText.text = _currentLevel.levelDescription;
    }
}
