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
    [ReadOnly] public List<GameThing> things;
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

    public static Level Deserialize(string levelString, bool isPreview = false)
    {
        Debug.Log(levelString);
        Level level = JsonUtility.FromJson<Level>(levelString);

        if (isPreview)
        {
            level.things.Clear();
            level.groundTiles.Clear();
            level.propTiles.Clear();
            level.objectTiles.Clear();
            level.rooms.Clear();

            return level;
        }
        

        foreach (Room room in level.rooms)
            room.SetDiscovered(false);

        return level;
    }

    public static List<string> GetLevels()
    {
        List<string> levels = new List<string>();

        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Levels/"))
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Levels/");

        foreach (string level in System.IO.Directory.GetFiles(Application.persistentDataPath + "/Levels/"))
            levels.Add(System.IO.Path.GetFileNameWithoutExtension(level));

        return levels;
    }

    [System.Serializable]
    public struct Room
    {
        // A room is simply a bounding box that stores a list of the things within that bounding box.
        // As with HeroQuest, the room's purpose is to act as a sort of "fog of war".
        // When the player enters a room, the things found in that room are activated and revealed.

        public Vector3Int min, max;

        // A list of all the things in this room.
        public List<GameThing> things;

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

        public bool Contains(GameThing thing)
        {
            return Contains(thing.transform.position);
        }
    }
}
