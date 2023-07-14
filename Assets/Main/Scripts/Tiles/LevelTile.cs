using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Tile", menuName = "2D/Tiles/Level Tile")]
public class LevelTile : RuleTile
{
    public virtual string tileData
    {
        get => _tileData;
        set => _tileData = value;
    }
    [SerializeField] protected string _tileData;
}