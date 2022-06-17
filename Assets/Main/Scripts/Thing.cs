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

    #region Camera

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

    #endregion

    #region Things

        public List<Thing> overlappingThings, thingsInFront;

    #endregion

    #region Movement

        public int movesLeft;

        private Vector3 location, previousLocation;

        public GraphNode currentNode = null;

        public bool solid = true;
        protected bool moving;
        public float tileMoveTime = 0.2f, maxStepHeight = 0.5f;

        private Vector3 direction;

        // Move to a new node.
        public void MoveTo(Vector3 position, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (moving || movesLeft <= 0)
                return;

            GraphNode oldNode = currentNode;
            GraphNode newNode = Map.GetNode(thing:this, position:position, checkHeight:checkHeight, ignoreCollisions:ignoreCollisions);

            if (newNode != null && newNode != oldNode)
            {
                currentNode = newNode;
                location = (Vector3)newNode.position;
            }

            if (previousLocation != location)
            {
                if (!moving)
                {
                    direction = (previousLocation - location).normalized;

                    moving = true;
                    StartCoroutine(Movement());

                    UpdateThings();
                    Rotate(direction);

                    movesLeft--;
                    previousLocation = location;
                }
            }
            else
                moving = false;

            // Debug.Log("Moving to " + position);
        }

        public void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
        {
            MoveTo(position:location + direction, ignoreCollisions:ignoreCollisions, checkHeight:checkHeight);
        }

        private IEnumerator Movement()
        {
            Debug.Log("Moving " + name);
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

        private void UpdateThings()
        {
            foreach (Thing thing in overlappingThings)
                thing.overlappingThings.Remove(this);
            overlappingThings.Clear();

            foreach (Thing thing in thingsInFront)
                thing.thingsInFront.Remove(this);
            thingsInFront.Clear();

            if (currentNode != null)
            {
                foreach (Thing thing in Map.CheckForThingsAtPosition(node:currentNode))
                {
                    if (thing != this)
                    {
                        if (thing.overlappingThings.Contains(this))
                            continue;
                        else
                            thing.overlappingThings.Add(this);
                    }
                }

                foreach (Thing thing in Map.CheckForThingsAtPosition(thing:this, position:location + direction))
                {
                    if (thing != this)
                    {
                        if (thing.thingsInFront.Contains(this))
                            continue;
                        else
                            thing.thingsInFront.Add(this);
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

        // private bool rotating = false;

        public void Rotate(Vector3 direction)
        {
            switch (rotationBehavior)
            {
                case MovementRotationBehavior.None:
                    return;
                case MovementRotationBehavior.FullRotation:
                    gotoRotation = ((Mathf.Atan2(direction.x, direction.z)) * Mathf.Rad2Deg);
                    break;
                case MovementRotationBehavior.LeftRightRotation:
                    if (direction.x < 0)
                        gotoRotation = 0;
                    else if (direction.x > 0)
                        gotoRotation = 180;
                    break;
            }

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
}
