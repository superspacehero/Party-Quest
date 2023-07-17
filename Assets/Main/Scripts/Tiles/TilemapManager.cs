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

    [SerializeField] private TileTypeList tileTypeList;

    public Tilemap _groundTilemap;
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

        GameManager.instance.level.groundTiles = GetTilesFromMap(_groundTilemap).ToList();

        Level.SaveLevel(GameManager.instance.level, mapSlot);

        IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
        {
            foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
            {
                if (map.HasTile(pos))
                {
                    LevelTile tile = map.GetTile<LevelTile>(pos);

                    yield return new SavedTile
                    {
                        position = pos,
                        tileName = tile.name,
                        tileThing = tile.thing.thingPrefab
                    };
                }
            }
        }
    }

    [Button]

    public void CopyMap()
    {
        // Copy the map to the clipboard

        GameManager.instance.level.lightDirection = lightTransform.eulerAngles;

        GameManager.instance.level.groundTiles = GetTilesFromMap(_groundTilemap).ToList();

        Level.CopyLevel(GameManager.instance.level);

        IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
        {
            foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
            {
                if (map.HasTile(pos))
                {
                    LevelTile tile = map.GetTile<LevelTile>(pos);

                    yield return new SavedTile
                    {
                        position = pos,
                        tileName = tile.name,
                        tileThing = tile.thing.thingPrefab
                    };
                }
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
            _groundTilemap.SetTile(tile.position, tileType);

            if (tile.tileThing != null)
                _groundTilemap.GetTile<LevelTile>(tile.position).InstantiateThing(tile.tileThing);
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

        var maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
            map.ClearAllTiles();

        if (clearPathfinder)
            pathfinder.Scan();
    }


    [Button]
    private void ClearMap()
    {
        ClearMap(true);
    }

    [Button]
    void UpdateNavMesh()
    {
        _groundTilemap.CompressBounds();

        // #if !UNITY_EDITOR
        // Resize the pathfinder's graph to fit the map, then update it
        AstarPath.active.data.gridGraph.center.x = _groundTilemap.localBounds.center.x;
        AstarPath.active.data.gridGraph.center.z = _groundTilemap.localBounds.center.z;

        AstarPath.active.data.gridGraph.SetDimensions
        (
            Mathf.CeilToInt(_groundTilemap.localBounds.size.x + 2),
            Mathf.CeilToInt(_groundTilemap.localBounds.size.z + 2),
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
    public GameObject tileThing;
}
