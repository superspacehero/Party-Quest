using UnityEngine;

[CreateAssetMenu(fileName = "GameObjectList", menuName = "GameObject List", order = 1)]
public class GameObjectList : ScriptableObject
{
    public GameObjectCategory[] categories;

    public bool Find(string name, out GameObject gObject)
    {
        foreach (var category in categories)
        {
            foreach (var gameObject in category.gameObjects)
            {
                if (gameObject.name == name)
                {
                    gObject = gameObject;
                    return true;
                }
            }
        }

        gObject = null;

        return false;
    }
}

[System.Serializable]
public class GameObjectCategory
{
    public string name;
    public GameObject[] gameObjects;
}