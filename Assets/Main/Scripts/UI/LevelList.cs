using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelList : Menu
{
    private List<string> _levels = new List<string>();

    [SerializeField] private GameObject levelUIPrefab;
    private List<LevelUI> _levelUIs = new List<LevelUI>();

    [SerializeField] private Transform levelListContent;

    [SerializeField] private LevelUI levelPreview;
    [SerializeField] private Button newLevelButton;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    public override void OnEnable()
    {
        LoadLevels();
        base.OnEnable();
    }

    private void LoadLevels()
    {
        // What this function does:
        // 1) Check if the number of levels in the _levels list is the same as the number of levels in the Level.GetLevels() list
        // 2) If it is, then we don't need to do anything
        // 3) If it isn't, then we need to:
        // 3a) Update the _levels list to match the Level.GetLevels() list
        // 3b) If we have more level UIs than levels, then we need to destroy the extra level UIs
        // 3c) If we have more levels than level UIs, then we need to create new level UIs
        // 3d) When we have the same number of level UIs as levels, then we need to update all their level strings
        // 3e) Lastly, when going through the level UIs, if they have a selectable component, then we set the click event to SetLevelPreview
        // 4) Set the selectable to be selected when the menu is enabled - if there are levels, we pick the first. Otherwise, we pick the new level button

        if (_levels.Count != Level.GetLevels().Count)
        {
            _levels = Level.GetLevels();

            if (_levelUIs.Count > _levels.Count)
            {
                for (int i = _levelUIs.Count - 1; i >= _levels.Count; i--)
                {
                    Destroy(_levelUIs[i].gameObject);
                    _levelUIs.RemoveAt(i);
                }
            }
            else if (_levelUIs.Count < _levels.Count)
            {
                for (int i = _levelUIs.Count; i < _levels.Count; i++)
                {
                    if (Instantiate(levelUIPrefab, levelListContent).TryGetComponent(out LevelUI newLevelUI))
                        _levelUIs.Add(newLevelUI);
                }
            }

            foreach (var levelUI in _levelUIs)
            {
                levelUI.levelString = _levels[_levelUIs.IndexOf(levelUI)];
                
                if (levelUI.levelSelectable != null && levelUI.levelSelectable is Button)
                    (levelUI.levelSelectable as Button).onClick.AddListener(() => SetLevelPreview(_levels[_levelUIs.IndexOf(levelUI)]));
            }
        }

        if (_levels.Count > 0)
            objectToSelect = _levelUIs[0].levelSelectable.gameObject;
        else if (newLevelButton != null)
            objectToSelect = newLevelButton.gameObject;
    }

    public void SetLevelPreview(string levelString)
    {
        levelPreview.levelString = levelString;
    }
}
