using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedObject : MonoBehaviour
{
    public static InstancedObject instance;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    protected virtual void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;

        Debug.Log("InstancedObject: " + instance.name, instance);
    }
}
