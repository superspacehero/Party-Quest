using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    #region Singleton

    public static LevelEditor instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<LevelEditor>();

            return _instance;
        }
    }
    private static LevelEditor _instance;

    #endregion

    public Menu tileListMenu, objectListMenu;
}