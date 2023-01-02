using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Dice : GameThing
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

        [SerializeField, FoldoutGroup("Attached Things")] protected Inventory.ThingSlot cameraPoint;

    #endregion

    #region Movement

        private bool rolled;
        private Rigidbody rb
        {
            get
            {
                if (_rb == null)
                    attachedThing.transform.TryGetComponent(out _rb);

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
        private void OnEnable()
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
                cameraPointOffset = cameraPoint.transform.localPosition;
                initialized = true;
            }

            rolled = false;
            thingValue = 0;
            cameraPoint.transform.localPosition = cameraPointOffset;
            rollTime = 0f;

            enabledEvent.Invoke();

            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.angularVelocity = Vector3.zero;
            
            attachedThing.transform.localPosition = Vector3.zero;
            attachedThing.transform.localRotation = Quaternion.identity;

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

                cameraPoint.transform.position = attachedThing.transform.position + cameraPointOffset;

                yield return General.waitForFixedUpdate;
                rollTime += Time.fixedDeltaTime;
            }

            // Dice behavior when it's stopped rolling.
            var upDir = dirs.OrderBy(x => Vector3.Angle(attachedThing.transform.TransformDirection(x), Vector3.up)).First();

            thingValue = dirs.IndexOf(upDir) + 1;
            onValueChanged.Invoke(thingValue);

            Debug.Log($"Value: {thingValue}");
        }

        private void Launch(Vector3 direction)
        {
            if (rolled && thingValue <= 0)
                return;

            if (thingValue > 0)
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

        public override void Use(GameThing user)
        {
            RandomLaunch();
        }

    #endregion
}
