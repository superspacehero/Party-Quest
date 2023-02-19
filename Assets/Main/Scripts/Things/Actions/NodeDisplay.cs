using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NodeDisplay : MonoBehaviour
{
    // The static instance of the class
    public static NodeDisplay instance;

    // The prefab to use for the node display
    public GameObject nodePrefab;

    // The parent transform to use for the node display objects
    public Transform nodeParent;

    // A dictionary to store the node display objects, with the GraphNode as the key
    Dictionary<GraphNode, GameObject> nodeObjects;

    // A queue to store the inactive node display objects
    Queue<GameObject> inactiveObjects;

    private void Awake()
    {
        // Set the static instance
        instance = this;
    }

    // A function to display the nodes
    public void DisplayNodes(List<GraphNode> nodes)
    {
        // Initialize the dictionary and queue
        nodeObjects = new Dictionary<GraphNode, GameObject>();
        inactiveObjects = new Queue<GameObject>();

        // Iterate through the nodes
        foreach (GraphNode node in nodes)
        {
            // Get a node display object from the queue or create a new one
            GameObject nodeObject;
            if (inactiveObjects.Count > 0)
            {
                nodeObject = inactiveObjects.Dequeue();
            }
            else
            {
                nodeObject = Instantiate(nodePrefab, nodeParent);
            }

            // Add the node display object to the dictionary
            nodeObjects.Add(node, nodeObject);

            // Set the position of the node display object to the position of the GraphNode
            nodeObject.transform.position = (Vector3)node.position;

            // Activate the node display object
            nodeObject.SetActive(true);
        }
    }

    // A function to hide the nodes
    public void HideNodes()
    {
        // Iterate through the node display objects
        foreach (GameObject nodeObject in nodeObjects.Values)
        {
            // Deactivate the node display object and add it to the queue
            nodeObject.SetActive(false);
            inactiveObjects.Enqueue(nodeObject);
        }

        // Clear the dictionary
        nodeObjects.Clear();
    }

    public GameObject GetNodeObject(GraphNode node)
    {
        // Return the node display object for the GraphNode
        return (nodeObjects.ContainsKey(node)) ? nodeObjects[node] : null;
    }

    public void ColorNodeObject(GraphNode node, Color color)
    {
        // Get the node display object for the GraphNode,
        if (GetNodeObject(node) != null && GetNodeObject(node).transform.GetChild(0).TryGetComponent(out SpriteRenderer renderer))
        {
            // and set the color of the node display object        
            renderer.color = color;
        }
    }

    public void ColorNodeObjects(List<GraphNode> nodes, Color color, Color occupiedColor)
    {
        // Iterate through the GraphNodes
        foreach (GraphNode node in nodes)
        {
            // Color the node display object for the GraphNode
            if (MoveAction.CheckNodeOccupied(node))
                ColorNodeObject(node, occupiedColor);
            else
                ColorNodeObject(node, color);
        }
    }
}