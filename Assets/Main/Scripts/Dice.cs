using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Dice : Thing
{
    #region Dice

        [SerializeField, FoldoutGroup("Dice")]
        private UnityEvent enabledEvent, disabledEvent;

        [SerializeField, FoldoutGroup("Dice"),]
        private UnityEvent<int> onValueChanged = new UnityEvent<int>();

        [SerializeField, FoldoutGroup("Dice"),]
        List<Vector3> dirs = new List<Vector3>(new Vector3[]
        {
            Vector3.up,
            Vector3.right,
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.down
        });

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
        private float stickLaunchMagnitude = 0.99f, verticalLaunchForce = 5f, launchForce = 10f, rollSpeed = 500f;

        private Vector3 rotationDirection;
        
        #region Initialization
        
            private Vector3 cameraPointOffset;
            private bool initialized;

        #endregion

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            if (!initialized)
            {
                cameraPointOffset = cameraPoint.localPosition;
                initialized = true;
            }

            rolled = false;
            value = 0;
            cameraPoint.localPosition = cameraPointOffset;

            meshBase.localPosition = Vector3.zero;
            meshBase.localRotation = Quaternion.identity;
            rb.constraints = RigidbodyConstraints.FreezePosition;
        }

        /// <summary>
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate()
        {
            if (!rolled)
            {
                rotationDirection.x = Mathf.PingPong(Time.time * rollSpeed, 180f);
                rotationDirection.y = Mathf.Repeat(Time.time * rollSpeed, 360f);
                
                // Roll the dice around to show all sides.
                rb.AddTorque(rotationDirection, ForceMode.Impulse);

                return;
            }

            if (value > 0)
                return;

            cameraPoint.position = meshBase.position + cameraPointOffset;

            if (rb.angularVelocity.magnitude < 0.01f)
            {
                var upDir = dirs.OrderBy(x => Vector3.Angle(meshBase.TransformDirection(x), Vector3.up)).First();

                value = 1 + dirs.IndexOf(upDir);
                onValueChanged.Invoke(value);
            }
        }

        private void Launch(Vector3 direction)
        {
            if (rolled)
                return;

            rolled = true;

            direction.Normalize();

            rb.constraints = RigidbodyConstraints.None;

            rb.AddForce((direction * launchForce) + (Vector3.up * verticalLaunchForce), ForceMode.Impulse);
            
            rb.AddTorque(((Vector3.right * direction.z) + (Vector3.back * direction.x)) * launchForce, ForceMode.Impulse);
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

        public override void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
        {
            if (direction.magnitude >= stickLaunchMagnitude)
                Launch(direction);
        }

        public override void PrimaryAction()
        {
            RandomLaunch();
        }

        public override void SecondaryAction()
        {
            RandomLaunch();
        }

    #endregion
}
