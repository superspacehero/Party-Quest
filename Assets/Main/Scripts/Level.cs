using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct Level
{
    public string levelName, levelDescription, levelPreview, levelAuthorID;
    public List<QuestGoal> mainQuests, sideQuests;

    [ReadOnly]
    public Vector3 lightDirection;

    [ReadOnly]
    public List<SavedTile> groundTiles, propTiles, objectTiles;

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
        // var builder = new StringBuilder(levelName + "\n");

        // builder.Append("ground[");
        // foreach (var groundTile in groundTiles)
        //     builder.Append($"{(int)groundTile.tileType.tileType}({groundTile.position.x},{groundTile.position.y},{groundTile.position.z})");
        // builder.Append("]\n");

        // builder.Append("prop[");
        // foreach (var propTile in propTiles)
        //     builder.Append($"{(int)propTile.tileType.tileType}({propTile.position.x},{propTile.position.y})");
        // builder.Append("]\n");

        // builder.Append("object[");
        // foreach (var objectTile in objectTiles)
        //     builder.Append($"{(int)objectTile.tileType.tileType}({objectTile.position.x},{objectTile.position.y})");
        // builder.Append("]\n");

        // return builder.ToString();
    }

    public static Level Deserialize(string levelString)
    {
        Debug.Log(levelString);
        return JsonUtility.FromJson<Level>(levelString);
    }
}
