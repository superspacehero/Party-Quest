using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Map : MonoBehaviour
{
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

    // Scan the map for nodes
    public void Scan()
    {
        pathfinder.Scan();
    }
}
