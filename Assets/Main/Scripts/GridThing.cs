using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

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

                if (useUI && Counter.instance != null)
                    Counter.instance.count = _movesLeft;
            }
        }
        [SerializeField]
        private int _movesLeft;

        public void AddMoves(int moves)
        {
            movesLeft += moves;
        }

        public GraphNode currentNode = null, nodeInFront = null;
        protected Vector3 location, previousLocation;
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
                    StartCoroutine(Movement());

                    if (movesLeft > 0)
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
            if (controlledThing)
            {
                controlledThing.Move(direction, ignoreCollisions, checkHeight);
                return;
            }

            if (movesLeft == 0)
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
            moving = true;

            // Debug.Log("Moving " + name);
            float time = 0;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = location;

            // Should I jump?
            if (startPosition.y != endPosition.y)
            {
                grounded = false;

                Vector3 currentPosition = startPosition;
                AnimationCurve curve = new AnimationCurve(
                    new Keyframe(0, startPosition.y),
                    new Keyframe(0.5f, Mathf.Max(startPosition.y, endPosition.y) + jumpHeight, -1, 1),
                    new Keyframe(1, endPosition.y));

                float calculatedTime = time;


                while (time < tileJumpTime)
                {
                    time += Time.deltaTime;
                    calculatedTime = time / tileJumpTime;

                    currentPosition = Vector3.Lerp(startPosition, endPosition, calculatedTime);
                    currentPosition.y = curve.Evaluate(calculatedTime);
                    verticalMovement = (calculatedTime) < 0.5f ? 1 : -1;

                    Debug.Log("Jump time: " + calculatedTime);

                    transform.position = currentPosition;
                    
                    yield return null;
                }
            }
            else
            {
                while (time < tileMoveTime)
                {
                    time += Time.deltaTime;
                    transform.position = Vector3.Lerp(startPosition, endPosition, time / tileMoveTime);
                    
                    yield return null;
                }
            }

            transform.position = endPosition;
            moving = false;
            grounded = true;
            verticalMovement = 0;
        }

        private void UpdateThingLists()
        {
            // Debug.Log("Updating thing lists");

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

        [FoldoutGroup("Things")]
        public List<GridThing> overlappingThings, thingsInFront;

    #endregion

    #region Actions

        public override void PrimaryAction()
        {
            if (controlledThing != null)
                controlledThing.PrimaryAction();
            else
                movesLeft = Random.Range(1, 10);
        }

        public override void SecondaryAction()
        {
            if (controlledThing != null)
                controlledThing.SecondaryAction();
            else
                movesLeft = Random.Range(1, 10);
        }

    #endregion

}
