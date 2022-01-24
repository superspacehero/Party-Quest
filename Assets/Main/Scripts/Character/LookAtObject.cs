using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public General.Priority lookAtPriority = General.Priority.lowPriority;

    public Transform lookAtPoint
    {
        get
        {
            if (_lookAtPoint == null)
                _lookAtPoint = transform;
            return _lookAtPoint;
        }
    }
    [SerializeField]
    private Transform _lookAtPoint;
}
