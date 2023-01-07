using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MoveAction : ActionThing
{
    // The number of spaces the character can move
    public int movementRange = 3;

    // The grid graph to use for pathfinding
    public GridGraph gridGraph;

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
        // Calculate the set of valid grid spaces within the number of spaces the character can move
        validSpaces = new List<GraphNode>();

        currentPosition = gridGraph.GetNearest(user.transform.position).node;
        validSpaces.Add(currentPosition);

        GraphNode currentNode = currentPosition;

        // Iterate through all the nodes in the movement range
        for (int i = 0; i < movementRange; i++)
        {
            // Iterate through all the directions
            foreach (Vector3 direction in directions)
            {
                // Get the node in the direction
                GraphNode node = gridGraph.GetNearest((Vector3)currentNode.position + direction).node;

                // If the node is not in the valid spaces list and is walkable, add it
                if (!validSpaces.Contains(node) && node.Walkable)
                {
                    validSpaces.Add(node);
                }
            }
        }

        // Display the valid grid spaces
        NodeDisplay.instance.DisplayNodes(validSpaces);

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
            yield return null;
        }

        // The action is no longer running
        actionRunning = false;

        // Hide the valid grid spaces
        NodeDisplay.instance.HideNodes();
    }
}
