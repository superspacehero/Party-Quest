using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Dice : MovingThing
{
    #region Dice

        [SerializeField, FoldoutGroup("Dice")]
        private UnityEvent enabledEvent, disabledEvent;

        [SerializeField, FoldoutGroup("Dice")]
        private UnityEvent<int> onValueChanged = new UnityEvent<int>();

        [SerializeField, FoldoutGroup("Dice")]
        List<Vector3> dirs = new List<Vector3>(new Vector3[]
        {
            Vector3.up,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.down
        });
        private Vector3 side;

        // [SerializeField, FoldoutGroup("Dice")]
        // private float Kp = 100f, Kd = 20f;
        [SerializeField, FoldoutGroup("Dice")]
        private LayerMask layerMask;

    #endregion

    #region Movement

        private bool rolled;
        private Rigidbody rb
        {
            get
            {
                if (_rb == null)
                    meshBase.TryGetComponent(out _rb);

                return _rb;
            }
        }
        private Rigidbody _rb;

        [SerializeField, FoldoutGroup("Dice"), Min(0f)]
        private float stickLaunchMagnitude = 0.99f, verticalLaunchForce = 5f, launchForce = 10f, rollSpeed = 500f, maxRollTime = 4f;

        private float rollTime = 0f;
        
        #region Initialization
        
            private Vector3 cameraPointOffset;
            private bool initialized;

        #endregion

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable()
        {
            InitializeDice();
        }

        void InitializeDice()
        {
            StartCoroutine(RollDice());
        }

        IEnumerator RollDice()
        {
            if (!initialized)
            {
                cameraPointOffset = cameraPoint.localPosition;
                initialized = true;
            }

            rolled = false;
            value = 0;
            cameraPoint.localPosition = cameraPointOffset;
            rollTime = 0f;

            enabledEvent.Invoke();

            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.angularVelocity = Vector3.zero;
            
            meshBase.localPosition = Vector3.zero;
            meshBase.localRotation = Quaternion.identity;

            // Spin dice until it's rolled.
            while (!rolled)
            {
                rotationDirection.x = Mathf.PingPong(Time.time * rollSpeed, 180f);
                rotationDirection.y = Mathf.Repeat(Time.time * rollSpeed, 360f);
                
                // Roll the dice around to show all sides.
                rb.AddTorque(rotationDirection, ForceMode.Impulse);

                yield return General.waitForFixedUpdate;
            }

            // Dice behavior while rolling.
            while (rollTime < maxRollTime && rb.angularVelocity.magnitude >= 0.01f)
            {
                // if (Physics.Raycast(rb.position, Physics.gravity, out RaycastHit hit, 1f, layerMask))
                // {
                //     Quaternion.FromToRotation(rb.rotation * side, hit.normal).ToAngleAxis(out float angle, out Vector3 axis);
                //     Vector3 err = Mathf.Deg2Rad * angle * axis;
                //     rb.AddTorque(Kp * err - Kd * rb.angularVelocity, ForceMode.Acceleration);
                // }

                cameraPoint.position = meshBase.position + cameraPointOffset;

                yield return General.waitForFixedUpdate;
                rollTime += Time.fixedDeltaTime;
            }

            // Dice behavior when it's stopped rolling.
            var upDir = dirs.OrderBy(x => Vector3.Angle(meshBase.TransformDirection(x), Vector3.up)).First();

            value = dirs.IndexOf(upDir) + 1;
            onValueChanged.Invoke(value);

            Debug.Log($"Value: {value}");
        }

        private void Launch(Vector3 direction)
        {
            if (rolled && value <= 0)
                return;

            if (value > 0)
                InitializeDice();

            rolled = true;

            side = dirs[Random.Range(0, dirs.Count)];

            direction.Normalize();

            rb.transform.Rotate(Random.Range(0, 4) * 90f, Random.Range(0, 4) * 90f, 0, Space.Self);

            rb.constraints = RigidbodyConstraints.None;

            rb.AddForce((direction * launchForce) + (Vector3.up * verticalLaunchForce), ForceMode.Impulse);
            
            rb.AddTorque(((Vector3.right * direction.z) + (Vector3.back * direction.x)) * rollSpeed, ForceMode.Impulse);
        }

    #endregion

    #region Actions

        void RandomLaunch()
        {
            Vector3 launchDirection = Random.insideUnitCircle.normalized;
            launchDirection.z = launchDirection.y;
            launchDirection.y = 0;

            Launch(launchDirection);
        }

        public override void Move(Vector3 direction, bool rotate = true, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (direction.magnitude >= stickLaunchMagnitude)
                Launch(direction);
        }

        public override void PrimaryAction(bool runningAction)
        {
            if (runningAction)
                RandomLaunch();
        }

        public override void SecondaryAction(bool runningAction)
        {
            if (runningAction)
                RandomLaunch();
        }

    #endregion
}
