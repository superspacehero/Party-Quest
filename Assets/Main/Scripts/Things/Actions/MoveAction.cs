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
    private int movementRange = 3;

    // The direction the character is moving in
    private Vector3 movement;

    // The position on the grid the character is currently located at
    public GraphNode currentNode;

    // The set of valid grid spaces within the movement range
    List<GraphNode> validSpaces;

    private Vector3Int[] directions = new Vector3Int[]
    {
        Vector3Int.forward,
        Vector3Int.back,
        Vector3Int.left,
        Vector3Int.right
    };

    protected override IEnumerator RunAction()
    {
        // Enable movement control
        user.TryGetComponent(out MovementController controller);

        if (controller != null)
        {
            controller.canControl = true;

            controller.canMove = true;
            controller.canJump = true;
        }

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        currentNode = Nodes.gridGraph.GetNearest(user.transform.position).node;
        Nodes.UnoccupyNode(currentNode);
        validSpaces = Nodes.GetNodesInRadius(user.transform.position, movementRange);

        // Display the valid grid spaces
        if (Nodes.instance != null)
        {
            Nodes.instance.DisplayNodes(validSpaces);

            Nodes.instance.ColorNodeObjects(validSpaces);

            Nodes.instance.ColorNodeObject(currentNode, Nodes.instance.currentColor);
        }
        else
            Debug.LogWarning("NodeDisplay is null");

        // The previous position of the user
        Vector3 previousPosition = user.transform.position;

        GraphNode previousNode = currentNode;

        // While the user is still moving and hasn't stopped their movement turn
        while (actionRunning)
        {
            // If the user has moved
            if (user.transform.position != previousPosition)
            {
                // Update currentPosition to the grid-based position of the user
                currentNode = Nodes.gridGraph.GetNearest(user.transform.position).node;

                // If the current position is not a valid space
                if (!validSpaces.Contains(currentNode))
                {
                    movement = user.transform.position - previousPosition;

                    // check the node in the direction of the x axis
                    GraphNode xNode = Nodes.gridGraph.GetNearest(previousPosition + Vector3.right * movement.x).node;

                    // check the node in the direction of the z axis
                    GraphNode zNode = Nodes.gridGraph.GetNearest(previousPosition + Vector3.forward * movement.z).node;

                    // revert the appropriate axes to the previous position
                    user.transform.position = new Vector3(!validSpaces.Contains(xNode) ? previousPosition.x : user.transform.position.x, user.transform.position.y, !validSpaces.Contains(zNode) ? previousPosition.z : user.transform.position.z);

                    currentNode = previousNode;
                }
                else if (Nodes.CheckNodeOccupied(currentNode))
                {
                    currentNode = previousNode;
                }

                if (previousNode != currentNode)
                {
                    // Highlight the current node
                    if (Nodes.instance != null)
                    {
                        Nodes.instance.ColorNodeObject(previousNode, Nodes.instance.walkableColor);
                        Nodes.instance.ColorNodeObject(currentNode, Nodes.instance.currentColor);
                    }
                }

                // Update the previous position
                previousNode = currentNode;
                previousPosition = user.transform.position;
            }

            // Wait until the next frame
            yield return General.waitForFixedUpdate;
        }

        // Hide the valid grid spaces
        Nodes.instance.HideNodes();

        // Disable movement control
        if (controller != null)
            controller.canControl = false;

        user.transform.position = (Vector3)currentNode.position;

        // Occupy the node
        Nodes.OccupyNode(currentNode);

        // The action is no longer running
        EndAction();
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
        {
            if (user is CharacterThing && user.TryGetComponent(out MovementController controller))
            {
                controller.canControl = !controller.canControl;
                (user as CharacterThing).DisplayInventory(!controller.canControl);
            }
        }
    }
}
