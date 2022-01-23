using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class Head : MonoBehaviour
{
    // This class is used to look at objects.
    // A list of high, medium, and low LookAtObjects is kept to randomly look at.
    // These objects are added by entering the trigger of the object.
    // There is also a top priority LookAtObject, and if that isn't null,
    // it takes priority over all other LookAtObjects.

    // Whatver object we want to look at, we set as the currentLookAtObject.

    private Transform rotatePoint
    {
        get
        {
            if (_rotatePoint == null)
                _rotatePoint = transform;

            return _rotatePoint;
        }
    }
    [SerializeField]
    private Transform _rotatePoint;

    public List<LookAtObject> highPriorityLookAtObjects, mediumPriorityLookAtObjects, lowPriorityLookAtObjects;
    public LookAtObject topPriorityLookAtObject;
    [SerializeField]
    private LookAtObject currentLookAtObject;

    private LookAtObject lookAtParent
    {
        get
        {
            if (_lookAtParent == null)
                _lookAtParent = transform.GetComponentInParent<LookAtObject>();
            return _lookAtParent;
        }
    }
    [SerializeField]
    private LookAtObject _lookAtParent;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // If currentLookAtObject exists, look at it.
        // Otherwise, look straight ahead.

        if (currentLookAtObject != null)
        {
            RotateTowards(currentLookAtObject.lookAtPoint.position);
        }
        else
        {
            RotateTowards(transform.forward, true);
        }
    }

    /// <summary>
    /// Rotates the object towards the given position.
    /// </summary>
    /// <param name="position">The position to look at.</param>
    /// <param name="relative">Whether the position is relative to the object.</param>
    void RotateTowards(Vector3 position, bool relative = false)
    {
        if (relative)
            rotatePoint.rotation = Quaternion.Lerp(rotatePoint.rotation, Quaternion.LookRotation(position.normalized), rotationTime * Time.deltaTime / rotationTime);
        else
            rotatePoint.rotation = Quaternion.Lerp(rotatePoint.rotation, Quaternion.LookRotation((transform.position - position).normalized), Time.deltaTime / rotationTime);
    }

    public float rotationTime = 0.1f;

    public void Look(LookAtObject lookObject = null)
    {
        if (lookObject != null)
        {
            currentLookAtObject = lookObject;
            return;
        }

        if (topPriorityLookAtObject != null)
        {
            currentLookAtObject = topPriorityLookAtObject;
            return;
        }
        
        if (highPriorityLookAtObjects.Count > 0)
        {
            currentLookAtObject = highPriorityLookAtObjects[Random.Range(0, highPriorityLookAtObjects.Count)];
            return;
        }

        if (mediumPriorityLookAtObjects.Count > 0)
        {
            currentLookAtObject = mediumPriorityLookAtObjects[Random.Range(0, mediumPriorityLookAtObjects.Count)];
            return;
        }

        if (lowPriorityLookAtObjects.Count > 0)
        {
            currentLookAtObject = lowPriorityLookAtObjects[Random.Range(0, lowPriorityLookAtObjects.Count)];
            return;
        }

        currentLookAtObject = null;
    }
    
    [Button]
    public void Look()
    {
        Look(null);
    }

    // If a LookAtObject enters the trigger, add it to the list of LookAtObjects.
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Head: OnTriggerEnter");
        if (other.TryGetComponent(out LookAtObject lookAtObject) && lookAtObject != lookAtParent)
        {
            if (lookAtObject.lookAtPriority == General.Priority.topPriority)
                topPriorityLookAtObject = lookAtObject;

            else if (lookAtObject.lookAtPriority == General.Priority.highPriority && !highPriorityLookAtObjects.Contains(lookAtObject))
                highPriorityLookAtObjects.Add(lookAtObject);
                
            else if (lookAtObject.lookAtPriority == General.Priority.mediumPriority && !mediumPriorityLookAtObjects.Contains(lookAtObject))
                mediumPriorityLookAtObjects.Add(lookAtObject);

            else if (lookAtObject.lookAtPriority == General.Priority.lowPriority && !lowPriorityLookAtObjects.Contains(lookAtObject))
                lowPriorityLookAtObjects.Add(lookAtObject);

            Look();
        }
    }

    // If a LookAtObject exits the trigger, remove it from the list of LookAtObjects.
    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LookAtObject lookAtObject))
        {
            if (lookAtObject.lookAtPriority == General.Priority.topPriority && topPriorityLookAtObject == lookAtObject)
                topPriorityLookAtObject = null;
            
            else if (lookAtObject.lookAtPriority == General.Priority.highPriority && highPriorityLookAtObjects.Contains(lookAtObject))
                highPriorityLookAtObjects.Remove(lookAtObject);
            
            else if (lookAtObject.lookAtPriority == General.Priority.mediumPriority && mediumPriorityLookAtObjects.Contains(lookAtObject))
                mediumPriorityLookAtObjects.Remove(lookAtObject);
            
            else if (lookAtObject.lookAtPriority == General.Priority.lowPriority && lowPriorityLookAtObjects.Contains(lookAtObject))
                lowPriorityLookAtObjects.Remove(lookAtObject);

            Look();
        }
    }
}
