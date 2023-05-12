using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Nodes : MonoBehaviour
{
    // The static instance of the class
    public static Nodes instance;

    // The grid graph to use for pathfinding
    public static GridGraph gridGraph
    {
        get
        {
            if (_gridGraph == null)
                _gridGraph = AstarPath.active.data.gridGraph;

            return _gridGraph;
        }
    }
    private static GridGraph _gridGraph;

    // The prefab to use for the node display
    public GameObject nodePrefab;

    // The parent transform to use for the node display objects
    public Transform nodeParent;

    // A dictionary to store the node display objects, with the GraphNode as the key
    Dictionary<GraphNode, GameObject> nodeObjects = new Dictionary<GraphNode, GameObject>();

    // A queue to store the inactive node display objects
    Queue<GameObject> inactiveObjects = new Queue<GameObject>();

    [SerializeField, Sirenix.OdinInspector.FoldoutGroup("Colors")]
    public Color walkableColor = Color.white, currentColor = Color.blue, occupiedColor = Color.red;

    private void Awake()
    {
        // Set the static instance
        instance = this;
    }

    // A function to display the nodes
    public void DisplayNodes(List<GraphNode> nodes)
    {
        HideNodes();

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

    public void DisplayNodes(Vector3 position, float radius, Vector2 maxHeightLimits)
    {
        // Get the nodes within the radius
        DisplayNodes(GetNodesInRadius(position, radius, maxHeightLimits));
    }

    private static Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.left,
        Vector3Int.right
    };

    public static List<GraphNode> GetNodesInRadius(Vector3 position, float radius, Vector2 maxHeightLimits)
    {
        List<GraphNode> nodes = new List<GraphNode>();

        GraphNode currentNode = gridGraph.GetNearest(position).node;
        nodes.Add(currentNode);

        Queue<GraphNode> queue = new Queue<GraphNode>();
        queue.Enqueue(currentNode);

        // Iterate through all the nodes in the movement range
        for (int i = 0; i < radius; i++)
        {
            int queueCount = queue.Count;
            for (int j = 0; j < queueCount; j++)
            {
                GraphNode searchNode = queue.Dequeue();
                // Iterate through all the directions
                foreach (Vector3 direction in directions)
                {
                    // Get the node in the direction
                    GraphNode node = gridGraph.GetNearest((Vector3)searchNode.position + direction).node;
                    // float deltaY = Mathf.Abs(node.position.y - searchNode.position.y);

                    // bool withinUpwardLimit = maxHeightLimits.x == -1 || deltaY <= maxHeightLimits.x;
                    // bool withinDownwardLimit = maxHeightLimits.y == -1 || deltaY <= maxHeightLimits.y;

                    // if (deltaY != 0)
                    //     Debug.Log($"Node: {node.position}, SearchNode: {searchNode.position}, DeltaY: {deltaY}, UpwardLimit: {withinUpwardLimit}, DownwardLimit: {withinDownwardLimit}");

                    // If the node is not in the valid spaces list, is walkable, and the height difference is within the specified limits, add it to the list
                    if (!nodes.Contains(node) && node.Walkable)// && withinUpwardLimit && withinDownwardLimit)
                    {
                        nodes.Add(node);
                        queue.Enqueue(node);
                    }
                }
            }
        }

        return nodes;
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

    public void ColorNodeObjects(List<GraphNode> nodes)
    {
        // Iterate through the GraphNodes
        foreach (GraphNode node in nodes)
        {
            // Color the node display object for the GraphNode
            if (CheckNodeOccupied(node))
                ColorNodeObject(node, occupiedColor);
            else
                ColorNodeObject(node, walkableColor);
        }
    }

    public static void OccupyNode(GraphNode node)
    {
        if (node != null)
            node.Tag = 1;
    }

    public static void OccupyNode(Vector3 position)
    {

        if (AstarPath.active != null)
            OccupyNode(AstarPath.active.data.gridGraph.GetNearest(position).node);
    }

    public static void UnoccupyNode(GraphNode node)
    {
        if (node != null)
            node.Tag = 0;
    }

    public static void UnoccupyNode(Vector3 position)
    {
        if (AstarPath.active != null)
            UnoccupyNode(AstarPath.active.data.gridGraph.GetNearest(position).node);
    }

    public static bool CheckNodeOccupied(GraphNode node)
    {
        return node.Tag == 1;
    }

    public static bool CheckNodeOccupied(Vector3 position, out CharacterThing outCharacter)
    {
        outCharacter = null;

        if (AstarPath.active != null)
        {
            GraphNode node = AstarPath.active.data.gridGraph.GetNearest(position).node;

            if (node.Tag == 1)
            {
                foreach (CharacterThing character in GameManager.characters)
                {
                    if (character.transform.position == (Vector3)node.position)
                    {
                        outCharacter = character;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}