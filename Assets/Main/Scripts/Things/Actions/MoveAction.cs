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

    [SerializeField, Sirenix.OdinInspector.FoldoutGroup("Colors")]
    private Color walkableColor = Color.white, currentColor = Color.blue, invalidColor = Color.red;

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
        validSpaces = new List<GraphNode>();

        currentNode = gridGraph.GetNearest(user.transform.position).node;
        validSpaces.Add(currentNode);

        Queue<GraphNode> queue = new Queue<GraphNode>();
        queue.Enqueue(currentNode);

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
        {
            NodeDisplay.instance.DisplayNodes(validSpaces);

            NodeDisplay.instance.ColorNodeObjects(validSpaces, walkableColor);

            NodeDisplay.instance.ColorNodeObject(currentNode, currentColor);
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
                currentNode = gridGraph.GetNearest(user.transform.position).node;

                // If the current position is not a valid space
                if (!validSpaces.Contains(currentNode))
                {
                    movement = user.transform.position - previousPosition;

                    // check the node in the direction of the x axis
                    GraphNode xNode = gridGraph.GetNearest(previousPosition + Vector3.right * movement.x).node;

                    // check the node in the direction of the y axis
                    GraphNode zNode = gridGraph.GetNearest(previousPosition + Vector3.forward * movement.z).node;

                    // revert the appropriate axes to the previous position
                    user.transform.position = new Vector3(!validSpaces.Contains(xNode) ? previousPosition.x : user.transform.position.x, user.transform.position.y, !validSpaces.Contains(zNode) ? previousPosition.z : user.transform.position.z);

                    currentNode = previousNode;
                }

                if (previousNode != currentNode)
                {
                    // Highlight the current node
                    if (NodeDisplay.instance != null)
                    {
                        NodeDisplay.instance.ColorNodeObject(previousNode, walkableColor);
                        NodeDisplay.instance.ColorNodeObject(currentNode, currentColor);
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
        NodeDisplay.instance.HideNodes();

        // Disable movement control
        if (controller != null)
            controller.canControl = false;

        user.transform.position = (Vector3)currentNode.position;

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
                (user as CharacterThing).DisplayActionList(!controller.canControl);
            }
        }
    }
}
