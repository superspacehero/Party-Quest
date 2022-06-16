using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Map : InstancedObject
{
    new protected static Map instance;

    public List<Thing> things;

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

        public static GraphNode GetNode(Thing thing, Vector3 position, bool checkHeight = true, bool ignoreCollisions = false)
        {
            GraphNode newNode = AstarPath.active.GetNearest(position).node;
            List<Thing> nodeThings = CheckForThingsAtPosition(newNode);

            if (newNode != null && newNode.Walkable)
            {
                if (checkHeight && Mathf.Abs(thing.transform.position.y - ((Vector3)newNode.position).y) > thing.maxStepHeight)
                    return null;

                if (nodeThings.Count > 0)
                {
                    if (!ignoreCollisions)
                    {
                        foreach (Thing thingInNode in nodeThings)
                            if (thingInNode != thing && thingInNode.solid)
                                return null;
                    }
                }
            }
            else
                return null;
            
            return newNode;
        }

        public static List<Thing> CheckForThingsAtPosition(GraphNode node)
        {
            if (node != null)
            {
                List<Thing> thingsAtPosition = new List<Thing>();

                foreach (Thing thing in instance.things)
                {
                    if (thing.currentNode == node)
                        thingsAtPosition.Add(thing);
                }

                return thingsAtPosition;
            }

            return null;
        }

        public static List<Thing> CheckForThingsAtPosition(Thing thing, Vector3 position)
        {
            GraphNode testNode = GetNode(thing, position, false, false);
            
            return CheckForThingsAtPosition(testNode);
        }

    #endregion
}
