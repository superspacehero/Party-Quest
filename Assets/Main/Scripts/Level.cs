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

    // Convert the Level struct to a JSON string
    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    // Create a Level struct from a JSON string
    public static Level Deserialize(string levelString, bool isPreview = false)
    {
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

    // Save a Level to a file
    public static void SaveLevel(Level level, int levelSlot)
    {
        string levelString = level.Serialize();
        System.IO.File.WriteAllText(Application.persistentDataPath + $"/Levels/Level_{levelSlot}.json", levelString);
    }

    // Copy a Level to the clipboard
    public static void CopyLevel(Level level)
    {
        string levelString = level.Serialize();
        GUIUtility.systemCopyBuffer = levelString;
    }

    // Load a Level from a file
    public static Level LoadLevel(int levelSlot)
    {
        string levelString = System.IO.File.ReadAllText(Application.persistentDataPath + $"/Levels/Level_{levelSlot}.json");
        Level level = Level.Deserialize(levelString);
        return level;
    }

    // Delete a Level file
    public static void DeleteLevel(int levelSlot)
    {
        System.IO.File.Delete(Application.persistentDataPath + $"/Levels/Level_{levelSlot}.json");
    }

    // Get a list of available Level files
    public static List<string> GetLevels()
    {
        string levelDirectory = Application.persistentDataPath + "/Levels/";

        if (!System.IO.Directory.Exists(levelDirectory))
            System.IO.Directory.CreateDirectory(levelDirectory);

        string[] levelFiles = System.IO.Directory.GetFiles(levelDirectory);
        List<string> levels = new List<string>();

        foreach (string levelFile in levelFiles)
        {
            string levelString = System.IO.File.ReadAllText(levelFile);
            levels.Add(levelString);
        }

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
