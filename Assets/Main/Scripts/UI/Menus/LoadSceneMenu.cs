using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class LoadSceneMenu : Menu
{
    [SerializeField, BoxGroup("Scene")] private bool loadOnSelect = true;
    [SerializeField, NaughtyAttributes.Scene, BoxGroup("Scene")] private string sceneName;

    public override void Select()
    {
        base.Select();

        if (loadOnSelect)
            LoadScene();
    }

    public void LoadScene()
    {
        General.LoadScene(sceneName);
    }
}