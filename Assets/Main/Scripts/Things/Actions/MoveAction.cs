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

    // The set of valid grid spaces within the movement range
    List<GraphNode> validSpaces;

    protected override IEnumerator RunAction()
    {
        int numberOfDiceRolls = (int)user.variables.GetVariable("movement"); // Get the number of dice rolls from user variables
        int sumOfDiceRolls = 0;

        yield return null;

        GameManager.EnableTouchControls();

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

            // Update the counter
            if (Counter.instance)
                Counter.instance.count = sumOfDiceRolls;

            user.DetachThing();
            GameManager.instance.dicePool.ReturnDieToPool(diceInstance);
        }

        movementRange = sumOfDiceRolls; // Set the movement range based on the sum of dice rolls

        // Enable movement control

        if (user is CharacterThing)
            (user as CharacterThing).input.canControl = true;

        user.TryGetComponent(out MovementController controller);
        if (controller != null)
        {
            controller.canControl = 2;

            controller.canMove = true;
            controller.canJump = true;

            jumpHeight = controller.jumpHeight.Value;
        }

        // Calculate the set of valid grid spaces within the number of spaces the character can move
        position = user.transform.position;
        Nodes.UnoccupyNode(currentNode);
        user.canOccupyCurrentNode = false;
        validSpaces = Nodes.GetNodesInRadius(user.transform.position, movementRange, -Vector2.one);

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
                // Update position to the grid-based position of the user
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

        user.transform.position = position;

        // Occupy the node
        Nodes.OccupyNode(currentNode);
        user.canOccupyCurrentNode = true;

        // Hide the counter
        if (Counter.instance)
            Counter.instance.count = 0;

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

                if (controller.canControl < 2)
                    (user as CharacterThing).DisplayActions(true);
                else
                    GameManager.EnableTouchControls();
            }
        }
    }
}
