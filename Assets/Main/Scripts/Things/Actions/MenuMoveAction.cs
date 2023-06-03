using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MenuMoveAction : StartingActionThing
{
    public override string thingSubType
    {
        get => "MenuMove";
    }

    // The direction the character is moving in
    private Vector3 movement;

    // The position on the grid the character is currently located at
    public GraphNode currentNode;

    // The set of valid grid spaces within the movement range
    List<GraphNode> validSpaces;

    protected override IEnumerator RunAction()
    {
        // Enable movement control
        user.TryGetComponent(out MovementController controller);

        if (controller != null)
        {
            controller.canControl = 2;

            controller.canMove = true;
            controller.canJump = true;
        }

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        currentNode = Nodes.gridGraph.GetNearest(user.transform.position).node;
        Nodes.UnoccupyNode(currentNode);
        validSpaces = Nodes.GetNodesInRadius(user.transform.position, float.PositiveInfinity, -Vector2.one);

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

                // Update the previous position
                previousNode = currentNode;
                previousPosition = user.transform.position;
            }

            // Wait until the next frame
            yield return General.waitForFixedUpdate;
        }

        // Disable movement control
        if (controller != null)
            controller.canControl = 0;

        user.transform.position = (Vector3)currentNode.position;

        // Occupy the node
        Nodes.OccupyNode(currentNode);

        // The action is no longer running
        EndAction();
    }

    protected override void EndAction(bool displayInventory = true)
    {
        actionRunning = false;
        onActionEnd?.Invoke();

        user.actionList.currentAction = null;

        GameManager.NextCharacter();
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
        {
            actionRunning = false;
        }
    }
}
