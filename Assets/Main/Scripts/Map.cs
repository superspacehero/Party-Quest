using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AstarPath))]
public class Map : MonoBehaviour
{
    public static List<GridThing> things = new List<GridThing>();

    private AstarPath pathfinder
    {
        get
        {
            if (_pathfinder == null)
                TryGetComponent(out _pathfinder);

            return _pathfinder;
        }
    }
    private AstarPath _pathfinder;

    // Scan the map for nodes
    public void Scan()
    {
        pathfinder.Scan();
    }

    #region Nodes and Things

        public static bool TestNodeWalkable(GraphNode node, GridThing referenceThing, bool checkHeight = true, bool ignoreCollisions = false)
        {
            if (node != null && node.Walkable)
            {
                if (checkHeight && Mathf.Abs(referenceThing.transform.position.y - ((Vector3)node.position).y) > referenceThing.maxStepHeight)
                    return false;

                List<GridThing> nodeThings = CheckForThingsAtPosition(node);

                if (nodeThings.Count > 0)
                {
                    if (!ignoreCollisions)
                    {
                        foreach (GridThing thingInNode in nodeThings)
                            if (thingInNode != referenceThing && thingInNode.solid)
                                return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static GraphNode GetNode(Vector3 position)
        {
            GraphNode newNode = AstarPath.active.GetNearest(position).node;
            if (newNode != null)
                return newNode;
            
            return null;            
        }

        public static List<GridThing> CheckForThingsAtPosition(GraphNode node)
        {
            if (node != null)
            {
                List<GridThing> thingsAtPosition = new List<GridThing>();

                foreach (GridThing thing in things)
                {
                    if (thing.currentNode == node)
                        thingsAtPosition.Add(thing);
                }

                return thingsAtPosition;
            }

            return null;
        }

        public static List<GridThing> CheckForThingsAtPosition(Vector3 position)
        {
            GraphNode testNode = GetNode(position);
            return CheckForThingsAtPosition(testNode);
        }

    #endregion
}
