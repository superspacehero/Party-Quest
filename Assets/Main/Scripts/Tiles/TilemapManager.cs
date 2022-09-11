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

    [SerializeField]
    private Level level;

    [SerializeField]
    private Tilemap _groundTilemap, _propTilemap, _objectTilemap;
    [SerializeField]
    private Transform lightTransform;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        LoadMap();
    }

    [Button]
    public void SaveMap()
    {
        // Save the map to a file

        level.lightDirection = lightTransform.eulerAngles;

        level.groundTiles = GetTilesFromMap(_groundTilemap).ToList();
        level.propTiles = GetTilesFromMap(_propTilemap).ToList();
        level.objectTiles = GetTilesFromMap(_objectTilemap).ToList();

        PlayerPrefs.SetString("TestLevel", JsonUtility.ToJson(level));
        
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
                        tileType = tile
                    };
                }
            }
        }
    }

    [Button]
    public void LoadMap()
    {
        // Load the map from a file
        ClearMap(false);

        level = Level.Deserialize(PlayerPrefs.GetString("TestLevel"));

        lightTransform.eulerAngles = level.lightDirection;

        foreach (SavedTile tile in level.groundTiles)
        {
            _groundTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in level.propTiles)
        {
            _propTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in level.objectTiles)
        {
            _objectTilemap.SetTile(tile.position, tile.tileType);
        }

        UpdateNavMesh();
    }

    public void ClearMap(bool clearPathfinder = true)
    {
        // Clear the map

        level.levelName = "";

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
                Mathf.CeilToInt(_groundTilemap.localBounds.size.x),
                Mathf.CeilToInt(_groundTilemap.localBounds.size.z),
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
    public LevelTile tileType;
}

[System.Serializable]
public struct Level
{
    public string levelName, levelDescription, levelPreview, levelAuthorID;

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
