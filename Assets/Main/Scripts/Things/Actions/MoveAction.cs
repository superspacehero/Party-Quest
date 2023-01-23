using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MoveAction : ActionThing
{
    public override string thingSubType
    {
        get => "Move";
    }

    // The number of spaces the character can move
    public int movementRange = 3;

    // The grid graph to use for pathfinding
    private GridGraph gridGraph
    {
        get
        {
            if (_gridGraph == null)
                _gridGraph = AstarPath.active.data.gridGraph;

            return _gridGraph;
        }
    }
    private GridGraph _gridGraph;

    // The position on the grid the character is currently located at
    public GraphNode currentPosition;

    // The set of valid grid spaces within the movement range
    List<GraphNode> validSpaces;

    private Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.left,
        Vector3Int.right
    };

    protected override IEnumerator RunAction(GameThing user)
    {
        // Enable movement control
        user.TryGetComponent(out MovementController controller);

        if (controller != null)
            controller.canMove = true;

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        validSpaces = new List<GraphNode>();

        currentPosition = gridGraph.GetNearest(user.transform.position).node;
        validSpaces.Add(currentPosition);

        Queue<GraphNode> queue = new Queue<GraphNode>();
        queue.Enqueue(currentPosition);

        // Iterate through all the nodes in the movement range
        for (int i = 0; i < movementRange; i++)
        {
            int queueCount = queue.Count;
            for (int j = 0; j < queueCount; j++)
            {
                GraphNode currentNode = queue.Dequeue();
                // Iterate through all the directions
                foreach (Vector3 direction in directions)
                {
                    // Get the node in the direction
                    GraphNode node = gridGraph.GetNearest((Vector3)currentNode.position + direction).node;
                    // If the node is not in the valid spaces list and is walkable, add it
                    if (!validSpaces.Contains(node) && node.Walkable)
                    {
                        validSpaces.Add(node);
                        queue.Enqueue(node);
                    }
                }
            }
        }

        // Display the valid grid spaces
        if (NodeDisplay.instance != null)
            NodeDisplay.instance.DisplayNodes(validSpaces);
        else
            Debug.LogWarning("NodeDisplay is null");

        // The previous position of the user
        Vector3 previousPosition = user.transform.position;

        // While the user is still moving and hasn't stopped their movement turn
        while (actionRunning)
        {
            // If the user has moved
            if (user.transform.position != previousPosition)
            {
                // Update the previous position
                previousPosition = user.transform.position;

                // Update currentPosition to the grid-based position of the user
                currentPosition = gridGraph.GetNearest(user.transform.position).node;

                // If the current position is not a valid space, snap back to the previous position
                if (!validSpaces.Contains(currentPosition))
                {
                    user.transform.position = previousPosition;
                }
            }

            // Wait until the next frame
            yield return General.waitForFixedUpdate;
        }

        // Hide the valid grid spaces
        NodeDisplay.instance.HideNodes();

        // Disable movement control
        if (controller != null)
            controller.canMove = false;

        // The action is no longer running
        EndAction();
    }
}
