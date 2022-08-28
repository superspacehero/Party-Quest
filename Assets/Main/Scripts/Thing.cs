using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;

public class Thing : LookAtObject
{
    // This class is used as the basis for all objects in the game.
    // It contains the basic properties of an object, such as its localized name,
    // its localized description, its icon, and its value.

    #region Metadata

        [FoldoutGroup("Info")]
        public LocalizedString nameString;
        [FoldoutGroup("Info")]
        public LocalizedString descriptionString;
        [FoldoutGroup("Info")]
        public Sprite icon;
        [FoldoutGroup("Info")]
        public int value;

    #endregion

    #region Other Variables

        [FoldoutGroup("Other")]
        public Health health;

        public Thing controlledThing
        {
            get => _controlledThing;
            set
            {
                _controlledThing = value;

                if (value != null)
                {
                    if (_controlledThing.moveCameraToMeWhenControlling && GameplayCamera.cameraObject == this)
                        GameplayCamera.SetCameraObject(_controlledThing);
                }
            }
        }
        [SerializeField, FoldoutGroup("Things")]
        protected Thing _controlledThing;

    #endregion
    
    #region Animation

        protected Animator anim
        {
            get
            {
                if (_anim == null)
                    TryGetComponent(out _anim);
                return _anim;
            }
        }
        private Animator _anim;

        protected int currentAnimationState;

        void UpdateAnimation()
        {
            previousPosition = transform.position;

            if (anim == null) return;
            var state = GetState();

            if (state == currentAnimationState) return;
            anim.CrossFade(state, 0.05f, 0);
            currentAnimationState = state;
        }

    
        protected bool grounded = true;
        private float animationLockedUntil;
        protected bool crouching, jumpTriggered, attacked, landed;
        protected int verticalMovement;


        private int GetState()
        {
            if (Time.time < animationLockedUntil) return currentAnimationState;

            // Priorities
            if (attacked) return LockState(General.Attack);
            if (crouching) return General.Crouch;
            if (landed) return LockState(General.Land);
            if (jumpTriggered) return General.Jump;

            if (grounded) return !moving ? General.Idle : General.Walk;
            return verticalMovement > 0 ? General.Jump : General.Fall;

            int LockState(int s)
            {
                animationLockedUntil = Time.time + anim.GetCurrentAnimatorStateInfo(0).length;
                return s;
            }
        }

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

        public bool moveCameraToMeWhenControlling;

        [ReadOnly]
        public bool useUI = false;

    #endregion

    #region Movement

        protected bool moving;
        public float tileMoveTime = 0.2f, tileJumpTime = 0.3f, maxStepHeight = 1f, jumpHeight = 0.5f;

        [ReadOnly]
        public Vector3 movement;
        protected Vector3 direction, previousPosition;

        // Move to a new node.
        public virtual void MoveTo(Vector3 position, bool ignoreCollisions = false, bool checkHeight = true)
        {
            direction = position - transform.position;
            transform.position = position;
        }

        public virtual void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (direction == Vector3.zero)
            {
                moving = false;
                return;
            }

            moving = true;

            if (controlledThing)
            {
                controlledThing.Move(direction, ignoreCollisions, checkHeight);
                // Rotate(direction);
            }
            else
            {
                MoveTo(transform.position + (direction * (Time.deltaTime / tileMoveTime)), ignoreCollisions, checkHeight);
                Rotate(direction);
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
        [SerializeField, FoldoutGroup("Rotation")]
        private MovementRotationBehavior rotationBehavior = MovementRotationBehavior.None;

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

        protected Vector3 rotationDirection = Vector3.right;

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

        public bool primaryAction, secondaryAction;

        public virtual void PrimaryAction(bool runningAction)
        {
            primaryAction = runningAction;

            if (controlledThing != null)
                controlledThing.PrimaryAction(runningAction);
        }

        public virtual void SecondaryAction(bool runningAction)
        {
            secondaryAction = runningAction;

            if (controlledThing != null)
                controlledThing.SecondaryAction(runningAction);
        }
        
        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate()
        {
            UpdateAnimation();
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
