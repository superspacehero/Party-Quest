using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SetStartScene
{
    private const string scenePath = "Assets/Main/Scenes/Menus_TitleCharacter.unity";

    static SetStartScene()
    {
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }
}