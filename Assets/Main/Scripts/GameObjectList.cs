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

    public bool Find(Vector2Int categoryAndObjectIndex, out GameObject gObject)
    {
        if (categoryAndObjectIndex.x < categories.Length && categoryAndObjectIndex.y < categories[(int)categoryAndObjectIndex.x].gameObjects.Length)
        {
            gObject = categories[(int)categoryAndObjectIndex.x].gameObjects[(int)categoryAndObjectIndex.y];
            return true;
        }

        gObject = null;

        return false;
    }

    public bool Find(GameObject gObject, out Vector2Int categoryAndObjectIndex)
    {
        for (int i = 0; i < categories.Length; i++)
        {
            for (int j = 0; j < categories[i].gameObjects.Length; j++)
            {
                if (categories[i].gameObjects[j] == gObject)
                {
                    categoryAndObjectIndex = new Vector2Int(i, j);
                    return true;
                }
            }
        }

        categoryAndObjectIndex = Vector2Int.zero;

        return false;
    }
}

[System.Serializable]
public class GameObjectCategory
{
    public string name;
    public GameObject[] gameObjects;
}