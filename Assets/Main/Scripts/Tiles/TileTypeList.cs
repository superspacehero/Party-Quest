using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TileTypeList", menuName = "ScriptableObjects/TileTypeList", order = 1)]
public class TileTypeList : ScriptableObject
{
    public List<LevelTile> tileTypes;
}