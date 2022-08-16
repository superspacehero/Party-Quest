using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap _groundTilemap, _objectTilemap;

    public void SaveMap()
    {
        // Save the map to a file
    }

    public void LoadMap()
    {
        // Load the map from a file
    }

    public void ClearMap()
    {
        // Clear the map
    }
}
