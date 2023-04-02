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
    public void SaveMap(int mapSlot = 0)
    {
        // Save the map to a file

        GameManager.instance.level.lightDirection = lightTransform.eulerAngles;

        GameManager.instance.level.groundTiles = GetTilesFromMap(_groundTilemap).ToList();
        GameManager.instance.level.propTiles = GetTilesFromMap(_propTilemap).ToList();
        GameManager.instance.level.objectTiles = GetTilesFromMap(_objectTilemap).ToList();
        
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

        PlayerPrefs.SetString($"Map_{mapSlot}", JsonUtility.ToJson(GameManager.instance.level));
        PlayerPrefs.Save();
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
            _groundTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in GameManager.instance.level.propTiles)
        {
            _propTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in GameManager.instance.level.objectTiles)
        {
            _objectTilemap.SetTile(tile.position, tile.tileType);
        }

        UpdateNavMesh();
    }

    [Button]
    public void LoadMap(int mapSlot)
    {
        LoadMap(PlayerPrefs.GetString($"Map_{mapSlot}"));
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

    public void LoadMap()
    {
        if (!string.IsNullOrEmpty(GameManager.levelString))
            LoadMap(GameManager.levelString);
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
