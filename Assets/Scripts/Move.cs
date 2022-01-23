using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Move : MonoBehaviour
{
    private Seeker seeker
    {
        get
        {
            if (_seeker == null)
                TryGetComponent(out _seeker);
            return _seeker;
        }
    }
    private Seeker _seeker;
    
    public Path path;
}
