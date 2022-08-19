using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

public class TilemapManager : MonoBehaviour
{
    [SerializeField]
    private string levelName;
    [SerializeField]
    private Tilemap _groundTilemap, _propTilemap, _objectTilemap;


    [Button]
    public void SaveMap()
    {
        // Save the map to a file
        Level newLevel = new Level();

        newLevel.levelName = levelName;

        newLevel.groundTiles = GetTilesFromMap(_groundTilemap).ToList();
        newLevel.propTiles = GetTilesFromMap(_propTilemap).ToList();
        newLevel.objectTiles = GetTilesFromMap(_objectTilemap).ToList();

        PlayerPrefs.SetString("TestLevel", JsonUtility.ToJson(newLevel));
        
        IEnumerable<SavedTile> GetTilesFromMap(Tilemap map)
        {
            foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
            {
                if (map.HasTile(pos))
                {
                    LevelTile tile = map.GetTile<LevelTile>(pos);
                    
                    yield return new SavedTile
                    {
                        position = new Vector3Int(pos.x, pos.y, tile.height),
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
        ClearMap();

        Level loadedLevel = Level.Deserialize(PlayerPrefs.GetString("TestLevel"));
        
        levelName = loadedLevel.levelName;

        foreach (SavedTile tile in loadedLevel.groundTiles)
        {
            _groundTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in loadedLevel.propTiles)
        {
            _propTilemap.SetTile(tile.position, tile.tileType);
        }

        foreach (SavedTile tile in loadedLevel.objectTiles)
        {
            _objectTilemap.SetTile(tile.position, tile.tileType);
        }
    }

    [Button]
    public void ClearMap()
    {
        // Clear the map

        levelName = "";

        var maps = FindObjectsOfType<Tilemap>();

        foreach (var map in maps)
            map.ClearAllTiles();
    }
}

[System.Serializable]
public class SavedTile
{
    public Vector3Int position;
    public LevelTile tileType;
}

public struct Level
{
    public string levelName;

    public List<SavedTile> groundTiles;
    public List<SavedTile> propTiles;
    public List<SavedTile> objectTiles;

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
