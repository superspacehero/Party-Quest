using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class MenuMoveAction : ActionThing
{
    public override string thingSubType
    {
        get => "MenuMove";
    }

    // The direction the character is moving in
    private Vector3 movement;

    // The set of valid grid spaces within the movement range
    List<GraphNode> validSpaces;

    protected override IEnumerator RunAction()
    {
        // Enable movement control

        if (user is CharacterThing)
            (user as CharacterThing).input.canControl = true;

        user.TryGetComponent(out MovementController controller);
        if (controller != null)
        {
            controller.canControl = MovementController.ControlLevel.Full;

            controller.canMove = true;
            controller.canJump = true;
        }

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        position = user.transform.position;
        user.UnoccupyCurrentNode(true);

        validSpaces = Nodes.GetNodesInRadius(user.transform.position, float.PositiveInfinity, -Vector2.one);

        // Nodes.instance.DisplayNodes(validSpaces);

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
                position = user.transform.position;

                // If the current position is not a valid space
                if (!validSpaces.Contains(currentNode))
                {
                    movement = user.transform.position - previousPosition;

                    // check the node in the direction of the x axis
                    GraphNode xNode = Nodes.instance.gridGraph.GetNearest(previousPosition + Vector3.right * movement.x).node;

                    // check the node in the direction of the z axis
                    GraphNode zNode = Nodes.instance.gridGraph.GetNearest(previousPosition + Vector3.forward * movement.z).node;

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
            controller.canControl = MovementController.ControlLevel.None;

        user.transform.position = position;

        // Occupy the node
        user.canOccupyCurrentNode = true;
        user.OccupyCurrentNode();

        // The action is no longer running
        EndAction();
    }

    protected override void EndAction(bool displayInventory = true)
    {
        actionRunning = false;
        onActionEnd?.Invoke();

        user.actionList.ClearAction();

        GameManager.NextCharacter();
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed && GameManager.players.Count > 1)
        {
            actionRunning = false;
        }
    }
}
