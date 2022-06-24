using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GridThing : Thing
{
    #region Movement

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            MoveTo(transform.position);
            Map.things.Add(this);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            Map.things.Remove(this);
        }

        public bool solid = true;

        public int movesLeft
        {
            get => _movesLeft;
            set
            {
                _movesLeft = value;

                if (_movesLeft < 0)
                    _movesLeft = 0;

                if (useUI)
                    Counter.instance.count = _movesLeft;
            }
        }
        private int _movesLeft;

        public void AddMoves(int moves)
        {
            movesLeft += moves;
        }

        public GraphNode currentNode = null, nodeInFront = null;
        private Vector3 location, previousLocation;
        Vector2 absoluteMovement = Vector2.zero;

        // Move to a new node.
        public override void MoveTo(Vector3 position, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (moving)
                return;

            GraphNode oldNode = currentNode;
            GraphNode newNode = Map.GetNode(position:position);

            if (newNode != oldNode && Map.TestNodeWalkable(newNode, this))
            {
                currentNode = newNode;
                location = (Vector3)newNode.position;
            }

            direction = (position - previousLocation).normalized;
            
            if (Map.GetNode(position:transform.position + direction) == nodeInFront)
                return;
            else
                nodeInFront = Map.GetNode(position:transform.position + direction);

            Rotate(direction);

            if (previousLocation != location)
            {
                if (!moving)
                {

                    moving = true;
                    StartCoroutine(Movement());

                    movesLeft--;
                    previousLocation = location;
                }
            }
            else
                moving = false;

            UpdateThingLists();
        }

        public override void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (movesLeft <= 0)
                return;

            absoluteMovement.x = Mathf.Abs(direction.x);
            absoluteMovement.y = Mathf.Abs(direction.z);

            // No diagonal movement allowed. If one axis is further than the other, move in that direction.

            if (absoluteMovement.x > absoluteMovement.y)
                direction.z = 0;
            else if (absoluteMovement.x < absoluteMovement.y)
                direction.x = 0;
            else
                return;

            MoveTo(position:location + direction, ignoreCollisions:ignoreCollisions, checkHeight:checkHeight);
        }

        private IEnumerator Movement()
        {
            // Debug.Log("Moving " + name);
            float time = 0;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = location;

            while (time < tileMoveTime)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPosition, time / tileMoveTime);
                yield return null;
            }

            transform.position = endPosition;
            moving = false;
        }

        private void UpdateThingLists()
        {
            Debug.Log("Updating thing lists");

            foreach (GridThing thing in overlappingThings)
                thing.overlappingThings.Remove(this);
            overlappingThings.Clear();

            foreach (GridThing thing in thingsInFront)
                thing.thingsInFront.Remove(this);
            thingsInFront.Clear();

            if (currentNode != null)
            {
                foreach (GridThing overlappingThing in Map.CheckForThingsAtPosition(node:currentNode))
                {
                    if (overlappingThing != this)
                    {
                        if (overlappingThing.overlappingThings.Contains(this))
                            continue;
                        else
                            overlappingThing.overlappingThings.Add(this);
                    }
                }

                foreach (GridThing thingInFront in Map.CheckForThingsAtPosition(nodeInFront))
                {
                    if (thingInFront != this)
                    {
                        if (thingInFront.thingsInFront.Contains(this))
                            continue;
                        else
                            thingInFront.thingsInFront.Add(this);
                    }
                }
            }
        }

    #endregion

    #region Things

        public List<GridThing> overlappingThings, thingsInFront;

    #endregion

    #region Actions

        public override void PrimaryAction()
        {
            movesLeft = Random.Range(1, 10);
        }

        public override void SecondaryAction()
        {
            movesLeft = Random.Range(1, 10);
        }

    #endregion

}
