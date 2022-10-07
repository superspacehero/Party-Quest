using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct Level
{
    public string levelName;
    [TextAreaAttribute] public string levelDescription;
    public string levelPreview, levelAuthorID;
    public List<QuestGoal> mainQuests, sideQuests;

    [ReadOnly]
    public Vector3 lightDirection;

    [ReadOnly]
    public List<SavedTile> groundTiles, propTiles, objectTiles;
    public List<Room> rooms;

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
        Level level = JsonUtility.FromJson<Level>(levelString);

        foreach (Room room in level.rooms)
            room.SetDiscovered(false);

        return level;
    }

    [System.Serializable]
    public struct Room
    {
        // A room is simply a bounding box that stores a list of the objects within that bounding box.
        // As with HeroQuest, the room's purpose is to act as a sort of "fog of war".
        // When the player enters a room, the objects found in that room are activated and revealed.

        public Vector3Int min, max;

        // A bool that determines whether or not the player has discovered this room. This shouldn't be serialized or saved.
        private bool discovered;
        public bool GetDiscovered()
        {
            return discovered;
        }
        public void SetDiscovered(bool isDiscovered)
        {
            discovered = isDiscovered;
        }

        public bool Contains(Vector3 position)
        {
            return position.x >= min.x && position.x <= max.x && position.y >= min.y && position.y <= max.y && position.z >= min.z && position.z <= max.z;
        }

        public bool Contains(Thing thing)
        {
            return Contains(thing.transform.position);
        }
    }
}
