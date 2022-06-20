using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;
using Pathfinding;

public class Thing : LookAtObject
{
    // This class is used as the basis for all objects in the game.
    // It contains the basic properties of an object, such as its localized name,
    // its localized description, its icon, and its value.

    [FoldoutGroup("Info")]
    public LocalizedString nameString;
    [FoldoutGroup("Info")]
    public LocalizedString descriptionString;
    [FoldoutGroup("Info")]
    public Sprite icon;
    [FoldoutGroup("Info")]
    public int value;

    #region Health

        public Health health;

    #endregion

    #region Camera/UI

        public Transform cameraPoint
        {
            get
            {
                if (_cameraPoint == null)
                    _cameraPoint = transform;

                return _cameraPoint;
            }
        }
        [SerializeField]
        private Transform _cameraPoint;

        [ReadOnly]
        public bool useUI = false;

    #endregion

    #region Things

        public List<Thing> overlappingThings, thingsInFront;

    #endregion

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

        private Vector3 location, previousLocation;

        public GraphNode currentNode = null, nodeInFront = null;

        public bool solid = true;
        protected bool moving;
        public float tileMoveTime = 0.2f, maxStepHeight = 0.5f;

        private Vector3 direction;

        // Move to a new node.
        public void MoveTo(Vector3 position, bool ignoreCollisions = false, bool checkHeight = true)
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

        Vector2 absoluteMovement = Vector2.zero;

        public void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
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

            foreach (Thing thing in overlappingThings)
                thing.overlappingThings.Remove(this);
            overlappingThings.Clear();

            foreach (Thing thing in thingsInFront)
                thing.thingsInFront.Remove(this);
            thingsInFront.Clear();

            if (currentNode != null)
            {
                foreach (Thing overlappingThing in Map.CheckForThingsAtPosition(node:currentNode))
                {
                    if (overlappingThing != this)
                    {
                        if (overlappingThing.overlappingThings.Contains(this))
                            continue;
                        else
                            overlappingThing.overlappingThings.Add(this);
                    }
                }

                foreach (Thing thingInFront in Map.CheckForThingsAtPosition(nodeInFront))
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

    #region Rotation

        public enum MovementRotationBehavior
        {
            None,
            FullRotation,
            LeftRightRotation
        }
        public MovementRotationBehavior rotationBehavior = MovementRotationBehavior.None;

        protected Transform meshBase
        {
            get
            {
                if (_meshBase == null)
                    _meshBase = transform;

                return _meshBase;
            }
        }
        [SerializeField, InfoBox("If left empty, the base object will be used."), FoldoutGroup("Rotation")]
        private Transform _meshBase;

        public float gotoRotation
        {
            get { return _gotoRotation; }

            set
            {
                if (_gotoRotation != value)
                    RotateTowardsGotoRotation();
                
                _gotoRotation = value;
            }
        }
        // [SerializeField, ReadOnly, FoldoutGroup("Rotation")]
        private float _gotoRotation;

        [FoldoutGroup("Rotation")]
        public float rotationTime = 0.25f;

        private Vector3 rotationDirection = Vector3.right;

        public void Rotate(Vector3 direction)
        {
            switch (rotationBehavior)
            {
                case MovementRotationBehavior.None:
                    return;
                case MovementRotationBehavior.FullRotation:
                    break;
                case MovementRotationBehavior.LeftRightRotation:
                    direction.z *= 0.5f;
                    if (direction.x == 0)
                        direction.x = rotationDirection.x;
                    break;
            }

            rotationDirection = direction;
            gotoRotation = ((Mathf.Atan2(rotationDirection.x, rotationDirection.z)) * Mathf.Rad2Deg) + (rotationBehavior == MovementRotationBehavior.LeftRightRotation ? -90 : 0);
        }

        [Button, FoldoutGroup("Rotation"), HideInEditorMode]
        public void RandomRotation()
        {
            gotoRotation = Random.Range(0f, 360f);
        }

        private void RotateTowardsGotoRotation()
        {
            StopCoroutine(Rotating());
            StartCoroutine(Rotating());
        }

        private IEnumerator Rotating()
        {
            float startTime = 0;
            // rotating = true;

            while (startTime < 1)
            {
                _meshBase.localEulerAngles = new Vector3(_meshBase.localEulerAngles.x, Mathf.LerpAngle(_meshBase.localEulerAngles.y, gotoRotation, rotationTime), _meshBase.localEulerAngles.z);
                startTime += Time.deltaTime / rotationTime;
                yield return null;
            }

            _meshBase.localEulerAngles = new Vector3(
                _meshBase.localEulerAngles.x,
                gotoRotation,
                _meshBase.localEulerAngles.z
            );

            // rotating = false;
        }

    #endregion

    #region Actions

        public void PrimaryAction()
        {
            movesLeft = Random.Range(1, 10);
        }

        public void SecondaryAction()
        {
            movesLeft = Random.Range(1, 10);
        }

    #endregion

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw an arrow pointing in the direction of, well, direction.
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * 1.5f);
    }
}
