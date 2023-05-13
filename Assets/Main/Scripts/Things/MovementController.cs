using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMF;
using Sirenix.OdinInspector;

//A very simplified controller script;
//This script is an example of a very simple walker controller that covers only the basics of character movement;
public class MovementController : Controller
{
    private Mover mover;
    float currentVerticalSpeed = 0f;
    bool isGrounded;

    public float movementSpeed = 7f;

    public float? jumpHeight
    {
        get
        {
            if (_jumpHeight == null)
                _jumpHeight = defaultJumpHeight;

            return _jumpHeight;
        }
        set
        {
            if (value > defaultJumpHeight)
                _jumpHeight = value;
            else
                _jumpHeight = defaultJumpHeight;
        }
    }
    private float? _jumpHeight = null;

    [SerializeField] private float defaultJumpHeight = 1f;
    [SerializeField, Min(0f)] private float jumpOffset = 0.15f;
    public float gravity = 10f;

    Vector3 lastVelocity = Vector3.zero;

    Rigidbody rb
    {
        get
        {
            if (_rb == null)
                TryGetComponent(out _rb);
            return _rb;
        }
    }
    Rigidbody _rb;

    public Transform cameraTransform;
    Transform tr;

    [FoldoutGroup("Controls"), Range(0, 2)]
    public int canControl = 0;
    [FoldoutGroup("Controls")]
    public bool canMove, canJump;

    [FoldoutGroup("Controls"), Apxfly.Editor.Attributes.Vector2Selector]
    public Vector2 movementInput;
    [FoldoutGroup("Controls")]
    public bool jumpInput;

    private bool isJumping;

    // Use this for initialization
    void Start()
    {
        tr = transform;
        TryGetComponent(out mover);

        //Set the camera transform;
        if (cameraTransform == null && GameplayCamera.instance != null)
            cameraTransform = GameplayCamera.instance.transform;
    }

    void FixedUpdate()
    {
        //Run initial mover ground check;
        mover.CheckForGround();

        //If character was not grounded int the last frame and is now grounded, call 'OnGroundContactRegained' function;
        if (isGrounded == false && mover.IsGrounded() == true)
            OnGroundContactRegained(lastVelocity);

        //Check whether the character is grounded and store result;
        isGrounded = mover.IsGrounded();

        Vector3 _velocity = Vector3.zero;

        //Add player movement to velocity;
        _velocity += CalculateMovementDirection() * movementSpeed;

        //Handle gravity;
        if (!isGrounded)
        {
            currentVerticalSpeed -= gravity * Time.deltaTime;
        }
        else
        {
            if (currentVerticalSpeed <= 0f)
                currentVerticalSpeed = 0f;
        }

        //Handle jumping;
        if (isGrounded && canControl > 0 && canJump && jumpInput && !isJumping)
        {
            OnJumpStart();
            
            currentVerticalSpeed = Mathf.Sqrt(2 * gravity * (jumpHeight.Value + jumpOffset));
            isGrounded = false;
            isJumping = true; // Set isJumping to true after jump starts
        }

        // Reset isJumping to false when character touches the ground
        if (!jumpInput && isGrounded)
        {
            isJumping = false;
        }

        //Add vertical velocity;
        _velocity += tr.up * currentVerticalSpeed;

        //Save current velocity for next frame;
        lastVelocity = _velocity;

        mover.SetExtendSensorRange(isGrounded);
        mover.SetVelocity(_velocity);

        //If the character is not allowed to control itself, freeze all rotation and horizontal position changes;
        if (rb)
            rb.constraints = (canControl > 0 && canMove) ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        //Rotate the character;
        RotateTowardsGotoRotation();
    }

    private Vector3 CalculateMovementDirection()
    {
        //If no character input script is attached to this object, return no input;
        if (canControl == 0 || !canMove)
            return Vector3.zero;

        Vector3 _direction = Vector3.zero;

        //If no camera transform has been assigned, use the character's transform axes to calculate the movement direction;
        if (cameraTransform == null)
        {
            _direction += tr.right * movementInput.x;
            _direction += tr.forward * movementInput.y;
        }
        else
        {
            //If a camera transform has been assigned, use the assigned transform's axes for movement direction;
            //Project movement direction so movement stays parallel to the ground;
            _direction += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * movementInput.x;
            _direction += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * movementInput.y;
        }

        //If necessary, clamp movement vector to magnitude of 1f;
        if (_direction.magnitude > 1f)
            _direction.Normalize();

        // Use this direction to rotate the character towards the movement direction;
        if (_direction.magnitude > 0.01f)
            Rotate(_direction);

        // Return the calculated movement direction;
        return _direction;
    }

    //This function is called when the controller has landed on a surface after being in the air;
    void OnGroundContactRegained(Vector3 _collisionVelocity)
    {
        //Call 'OnLand' delegate function;
        if (OnLand != null)
            OnLand(_collisionVelocity);
    }

    //This function is called when the controller has started a jump;
    void OnJumpStart()
    {
        //Call 'OnJump' delegate function;
        if (OnJump != null)
            OnJump(lastVelocity);
    }

    //Return the current velocity of the character;
    public override Vector3 GetVelocity()
    {
        return lastVelocity;
    }

    //Return only the current movement velocity (without any vertical velocity);
    public override Vector3 GetMovementVelocity()
    {
        return lastVelocity;
    }

    //Return whether the character is currently grounded;
    public override bool IsGrounded()
    {
        return isGrounded;
    }

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

        private float gotoRotation;

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
                    // Round direction.x to -1 or 1 if it's not 0
                    if (direction.x != 0)
                        direction.x = Mathf.Sign(direction.x);

                    direction = Vector3Int.RoundToInt(direction);

                    if (direction.x == 0)
                        direction.x = rotationDirection.x;

                    direction.z *= 0.5f;
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
            _meshBase.localEulerAngles = new Vector3(
                _meshBase.localEulerAngles.x,
                Mathf.LerpAngle(_meshBase.localEulerAngles.y, gotoRotation, Time.fixedDeltaTime / rotationTime),
                _meshBase.localEulerAngles.z
            );
        }

    #endregion
}