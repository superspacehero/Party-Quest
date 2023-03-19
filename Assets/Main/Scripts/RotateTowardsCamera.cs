using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RotateTowardsCamera : MonoBehaviour
{
    // /// <summary>
    // /// This function is called when the object becomes enabled and active.
    // /// </summary>
    // void OnEnable()
    // {
    //     GameplayCamera.cameraRotators.Add(this);
    // }

    // /// <summary>
    // /// This function is called when the behaviour becomes disabled or inactive.
    // /// </summary>
    // void OnDisable()
    // {
    //     GameplayCamera.cameraRotators.Remove(this);
    // }

    private Renderer referenceRenderer
    {
        get
        {
            if (_referenceRenderer == null)
                _referenceRenderer = GetComponentInChildren<Renderer>();

            return _referenceRenderer;
        }
    }
    [SerializeField, Tooltip("This is what we use to see if we're in the camera view or not.")]
    private Renderer _referenceRenderer;

    [Button]
    private void SetReferenceRenderer()
    {
        _referenceRenderer = referenceRenderer;
    }

    public bool CheckIfInCamera(Camera targetCamera)
    {
        return (referenceRenderer.IsVisibleFrom(targetCamera));
    }

    public void Rotate(Camera targetCamera)
    {
        if (!CheckIfInCamera(targetCamera))
            return;

        transform.eulerAngles = (Vector3.up * (targetCamera.transform.eulerAngles.y));
    }
}
