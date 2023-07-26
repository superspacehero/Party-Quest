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
    public List<SavedTile> groundTiles;

    public bool GetTile(GameThing thing, out SavedTile foundTile)
    {
        foundTile = null;
        
        foreach (SavedTile tile in groundTiles)
        {
            if (tile.tileThing == thing)
            {
                foundTile = tile;
                return true;
            }
        }

        return false;
    }

    public bool GetTile(Vector3 position, bool useWorldPosition, out SavedTile foundTile)
    {
        foundTile = null;
        
        foreach (SavedTile tile in groundTiles)
        {
            if (tile.position == ((useWorldPosition) ? TilemapManager.instance.tilemap.WorldToCell(position) : position))
            {
                foundTile = tile;
                return true;
            }
        }

        return false;
    }

    public void AddThing(GameThing thing)
    {
        foreach (Room room in rooms)
        {
            if (room.Contains(thing.transform.position))
            {
                room.things.Add(thing);
                return;
            }
        }

        if (rooms.Count > 0)
        {
            rooms[0].things.Add(thing);
            rooms[0].FitRoomToThings();
        }
        else
        {
            Room newRoom = new Room();
            newRoom.min = new Vector3Int(Mathf.FloorToInt(thing.transform.position.x), Mathf.FloorToInt(thing.transform.position.y), Mathf.FloorToInt(thing.transform.position.z));
            newRoom.max = new Vector3Int(Mathf.CeilToInt(thing.transform.position.x), Mathf.CeilToInt(thing.transform.position.y), Mathf.CeilToInt(thing.transform.position.z));
            newRoom.things = new List<GameThing>();
            newRoom.things.Add(thing);
            rooms.Add(newRoom);
        }

        // Debug.Log($"Added {thing.thingName}.", thing);
    }
    public void RemoveThing(GameThing thing)
    {
        if (groundTiles == null)
            return;

        if (GetTile(thing, out SavedTile foundTile))
        {
            foundTile.tileThing = null;
            return;
        }

        foreach (Room room in rooms)
        {
            if (room.Contains(thing, out GameThing foundThing))
            {
                room.things.Remove(foundThing);
                return;
            }
        }
    }

    [ReadOnly] public List<CharacterThing> characters;
    [ReadOnly] public List<CharacterSpawner> characterSpawners;
    [ReadOnly] public List<CharacterAndPosition> characterInfos;

    #region Character Spawners

        public struct CharacterAndPosition
        {
            public Vector3 position;
            public string characterInfo;
            public int team;

            public CharacterAndPosition(Vector3 position, string characterInfo, int team = 0)
            {
                this.position = position;
                this.characterInfo = characterInfo;
                this.team = team;
            }

            public CharacterAndPosition(Vector3 position, CharacterThing.CharacterInfo character, int team = 0)
            {
                this.position = position;
                this.characterInfo = character.ToString();
                this.team = team;
            }
        }

    #endregion

    public List<Room> rooms;

    // Convert the Level struct to a JSON string
    public string Serialize()
    {
        // Create an (almost) replica Level struct that will be used to create a JSON string
        Level level = new Level(levelName);
        level.levelDescription = levelDescription;
        level.levelPreview = levelPreview;
        level.levelAuthorID = levelAuthorID;
        level.mainQuests = mainQuests;
        level.sideQuests = sideQuests;
        level.lightDirection = lightDirection;
        level.groundTiles = groundTiles;
        level.characterInfos = characterInfos;
        level.rooms = rooms;


        return JsonUtility.ToJson(level);
    }

    // Create a Level struct from a JSON string
    public static Level Deserialize(string levelString, bool isPreview = false)
    {
        if (string.IsNullOrEmpty(levelString) || string.IsNullOrWhiteSpace(levelString))
            return new Level("");

        Level level = JsonUtility.FromJson<Level>(levelString);

        if (isPreview)
        {
            level.rooms.Clear();
            level.characters.Clear();
            level.groundTiles.Clear();

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

        public bool Contains(Vector3 position, out GameThing foundThing)
        {
            foundThing = null;

            foreach (GameThing thingInRoom in things)
            {
                if (thingInRoom.transform.position == position)
                {
                    foundThing = thingInRoom;
                    return true;
                }
            }

            return false;
        }

        public bool Contains(Vector3 position)
        {
            return position.x >= min.x && position.x <= max.x && position.y >= min.y && position.y <= max.y && position.z >= min.z && position.z <= max.z;
        }

        public bool Contains(GameThing thing, out GameThing foundThing)
        {
            foundThing = null;

            foreach (GameThing thingInRoom in things)
            {
                if (thingInRoom == thing)
                {
                    foundThing = thingInRoom;
                    return true;
                }
            }

            return false;
        }

        [Button]
        public void FitRoomToThings()
        {
            min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

            foreach (GameThing thing in things)
            {
                if (thing == null)
                    continue;

                if (thing.transform.position.x < min.x)
                    min.x = Mathf.FloorToInt(thing.transform.position.x);
                if (thing.transform.position.y < min.y)
                    min.y = Mathf.FloorToInt(thing.transform.position.y);
                if (thing.transform.position.z < min.z)
                    min.z = Mathf.FloorToInt(thing.transform.position.z);

                if (thing.transform.position.x > max.x)
                    max.x = Mathf.CeilToInt(thing.transform.position.x);
                if (thing.transform.position.y > max.y)
                    max.y = Mathf.CeilToInt(thing.transform.position.y);
                if (thing.transform.position.z > max.z)
                    max.z = Mathf.CeilToInt(thing.transform.position.z);
            }
        }
    }

    public Level(string newLevelName = "New Level")
    {
        levelName = newLevelName;
        levelDescription = "";
        levelPreview = "";
        levelAuthorID = "";
        mainQuests = new List<QuestGoal>();
        sideQuests = new List<QuestGoal>();
        lightDirection = Vector3.zero;
        groundTiles = new List<SavedTile>();
        characters = new List<CharacterThing>();
        characterSpawners = new List<CharacterSpawner>();
        characterInfos = new List<CharacterAndPosition>();
        rooms = new List<Room>();
    }
}
