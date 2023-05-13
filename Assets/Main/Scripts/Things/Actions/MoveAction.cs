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

    [SerializeField] private GameObject dicePrefab;

    // The number of spaces the character can move
    private int movementRange = 3;

    // The height the character can jump
    private float jumpHeight = 1;

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
        int numberOfDiceRolls = (int)user.variables.GetVariable("movement"); // Get the number of dice rolls from user variables
        int sumOfDiceRolls = 0;

        yield return null;

        for (int i = 0; i < numberOfDiceRolls; i++)
        {
            // Roll the dice to determine the movement range
            Dice diceInstance = GameManager.instance.dicePool.GetDieFromPool(user.thingTop.position, OnDiceRollStarted, OnDiceRollFinished);
            user.AttachThing(diceInstance);

            // Wait until the dice roll is finished
            while (!diceRollFinished)
            {
                yield return null;
            }

            sumOfDiceRolls += currentDiceValue; // Add the current dice value to the sum of dice rolls
            user.DetachThing();
            GameManager.instance.dicePool.ReturnDieToPool(diceInstance);
        }

        movementRange = sumOfDiceRolls; // Set the movement range based on the sum of dice rolls

        // Enable movement control
        user.TryGetComponent(out MovementController controller);

        if (controller != null)
        {
            controller.canControl = 2;

            controller.canMove = true;
            controller.canJump = true;

            jumpHeight = controller.jumpHeight.Value;
        }

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        currentNode = Nodes.gridGraph.GetNearest(user.transform.position).node;
        Nodes.UnoccupyNode(currentNode);
        validSpaces = Nodes.GetNodesInRadius(user.transform.position, movementRange, new Vector2(jumpHeight, -1));

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
            controller.canControl = 0;

        user.transform.position = (Vector3)currentNode.position;

        // Occupy the node
        Nodes.OccupyNode(currentNode);

        // The action is no longer running
        EndAction();
    }

    private bool diceRollFinished = false;
    private int currentDiceValue = 0;

    private void OnDiceRollStarted()
    {
        diceRollFinished = false;
    }

    private void OnDiceRollFinished(int diceValue)
    {
        currentDiceValue = diceValue;
        diceRollFinished = true;
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
        {
            if (user is CharacterThing && user.TryGetComponent(out MovementController controller))
            {
                controller.canControl = (controller.canControl == 0) ? 2 : 0;
                (user as CharacterThing).DisplayInventory(controller.canControl <= 1);
            }
        }
    }
}
