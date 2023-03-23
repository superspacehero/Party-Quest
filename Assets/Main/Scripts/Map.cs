using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(AstarPath))]
public class Map : MonoBehaviour
{
    public static List<GameThing> things = new List<GameThing>();

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

        public static bool TestNodeWalkable(GraphNode node, GameThing referenceThing, bool ignoreCollisions = false)
        {
            if (node != null && node.Walkable)
            {
                List<GameThing> nodeThings = CheckForThingsAtPosition(node);

                if (nodeThings.Count > 0)
                {
                    if (!ignoreCollisions)
                    {
                        foreach (GameThing thingInNode in nodeThings)
                            if (thingInNode != referenceThing && Nodes.CheckNodeOccupied(node))
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

        public static List<GameThing> CheckForThingsAtPosition(GraphNode node)
        {
            if (node != null)
            {
                List<GameThing> thingsAtPosition = new List<GameThing>();

                foreach (GameThing thing in things)
                {
                    if (GetNode(thing.transform.position) == node)
                        thingsAtPosition.Add(thing);
                }

                return thingsAtPosition;
            }

            return null;
        }

        public static List<GameThing> CheckForThingsAtPosition(Vector3 position)
        {
            GraphNode testNode = GetNode(position);
            return CheckForThingsAtPosition(testNode);
        }

    #endregion
}
