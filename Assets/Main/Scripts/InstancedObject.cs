using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancedObject : MonoBehaviour
{
    public InstancedObject instance;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    protected virtual void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("There is already an instance of " + GetType() + " in the scene. Destroying this one.");
            Destroy(this);
            return;
        }
        instance = this;

        Debug.Log("InstancedObject: " + instance.name + "of type " + instance.GetType());
    }
}
