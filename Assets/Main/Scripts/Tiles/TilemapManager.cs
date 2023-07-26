using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager instance;
    private AstarPath pathfinder
    {
        get
        {
            if (_pathfinder == null)
                TryGetComponent(out _pathfinder);
            return _pathfinder;
        }
    }
    private AstarPath _pathfinder;

    [SerializeField] private string levelOverrideString;

    public GameObjectList gameObjectList;
    public TileTypeList tileTypeList;

    public Tilemap tilemap;
    [SerializeField]
    private Transform lightTransform;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Make sure there is only one TilemapManager
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        LoadMap();
    }


    [Button]
    public void SaveMap(int mapSlot = 0)
    {
        // Save the map to a file

        GameManager.instance.level.lightDirection = lightTransform.eulerAngles;

        GameManager.instance.level.groundTiles = GetTilesFromMap(tilemap).ToList();

        Level.SaveLevel(GameManager.instance.level, mapSlot);
    }

    [Button]
    public void CopyMap()
    {
        // Copy the map to the clipboard

        GameManager.instance.level.lightDirection = lightTransform.eulerAngles;

        GameManager.instance.level.groundTiles = GetTilesFromMap(tilemap).ToList();

        Level.CopyLevel(GameManager.instance.level);
    }

    [Button]
    private void ListTileThings()
    {
        StringBuilder sb = new StringBuilder();

        foreach (SavedTile tile in GetTilesFromMap(tilemap))
        {
            if (!string.IsNullOrEmpty(tile.tileThingName))
                sb.AppendLine(tile.tileThingName);
        }

        Debug.Log(sb.ToString());
    }

    IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
    {
        // Get all the tiles from a map
        foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
        {
            if (map.HasTile(pos))
            {
                LevelTile tile = map.GetTile<LevelTile>(pos);

                yield return new SavedTile
                {
                    position = pos,
                    tileName = tile.name,
                    tileThingName = (GameManager.instance.level.GetTile(pos, false, out SavedTile savedTile)) ? savedTile?.tileThing?.thingPrefab?.name : ""
                };
            }
        }
    }

    [Button]
    public void LoadMap(string mapString)
    {
        // Load the map from a file
        ClearMap(false);

        GameManager.instance.level = Level.Deserialize(mapString);

        lightTransform.eulerAngles = GameManager.instance.level.lightDirection;

        foreach (SavedTile tile in GameManager.instance.level.groundTiles)
        {
            LevelTile tileType = tileTypeList.tileTypes.Find(t => t.name == tile.tileName);
            tilemap.SetTile(tile.position, tileType);

            if (!string.IsNullOrEmpty(tile.tileThingName) && gameObjectList.Find(tile.tileThingName, out GameObject thing))
                LevelTile.InstantiateThing(tile, thing, tilemap);
        }


        UpdateNavMesh();
    }

    [Button]
    public void LoadMap(int mapSlot)
    {
        LoadMap(Level.GetLevels()[mapSlot]);
    }

    public void LoadMap()
    {
        // If there is a level override string, load that
        if (!string.IsNullOrEmpty(levelOverrideString))
        {
            LoadMap(levelOverrideString);
            return;
        }

        // Load the map from a file
        if (!string.IsNullOrEmpty(GameManager.levelString))
        {
            LoadMap(GameManager.levelString);
        }
        else
        {
            List<string> levels = Level.GetLevels();
            if (levels.Count > 0)
            {
                int desiredLevelSlot = 0; // Change this value to the desired level slot
                LoadMap(levels[desiredLevelSlot]);
            }
        }
    }

    public void ClearMap(bool clearPathfinder = true)
    {
        // Clear the map

        GameManager.instance.level.levelName = "";
        GameManager.instance.level.levelDescription = "";

        tilemap.ClearAllTiles();

        for (int i = tilemap.transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(tilemap.transform.GetChild(i).gameObject);
#else
            Destroy(tilemap.transform.GetChild(i).gameObject);
#endif
        }

        if (clearPathfinder)
            pathfinder.Scan();
    }

    [Button]
    private void ClearMap()
    {
        ClearMap(true);
    }

    [Button]
    public void AddObjectToMap(GameObject thing, Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        cellPosition.z = Mathf.RoundToInt(position.y);

        // Add an object to the map
        if (GameManager.instance.level.GetTile(cellPosition, false, out SavedTile tile))
        {
            if (LevelTile.InstantiateThing(tile, thing, tilemap))
            {
                tilemap.RefreshTile(cellPosition);

                if (GameManager.gameMode != GameMode.Make)
                    UpdateNavMesh();
            }
        }
    }

    public void RemoveObjectFromMap(Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        cellPosition.z = Mathf.RoundToInt(position.y);

        // Remove an object from the map
        if (GameManager.instance.level.GetTile(cellPosition, false, out SavedTile tile))
        {
            if (tile.tileThing != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(tile.tileThing.gameObject);
#else
                Destroy(tile.tileThing.gameObject);
#endif
                tile.tileThing = null;
                tilemap.RefreshTile(cellPosition);

                if (GameManager.gameMode != GameMode.Make)
                    UpdateNavMesh();
            }
        }
    }

    [Button]
    void UpdateNavMesh()
    {
        tilemap.CompressBounds();

        // #if !UNITY_EDITOR
        // Resize the pathfinder's graph to fit the map, then update it
        AstarPath.active.data.gridGraph.center.x = tilemap.localBounds.center.x;
        AstarPath.active.data.gridGraph.center.z = tilemap.localBounds.center.z;

        AstarPath.active.data.gridGraph.SetDimensions
        (
            Mathf.CeilToInt(tilemap.localBounds.size.x + 2),
            Mathf.CeilToInt(tilemap.localBounds.size.z + 2),
            1
        );
        pathfinder.Scan();
        // #endif
    }
}

[System.Serializable]
public class SavedTile
{
    public Vector3Int position;
    public string tileName;
    public string tileThingName;

    public GameThing tileThing
    {
        get => _tileThing;
        set
        {
            _tileThing = value;
            if (value != null && value.thingPrefab != null)
                tileThingName = value.thingPrefab.name;
            else
                tileThingName = "";
        }
    }
    private GameThing _tileThing;
}
