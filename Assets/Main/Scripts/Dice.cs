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
        public UnityEvent enabledEvent, disabledEvent;

        [SerializeField, FoldoutGroup("Dice")]
        public UnityEvent<int> onValueChanged = new UnityEvent<int>();

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

        [SerializeField, FoldoutGroup("Dice")] protected Transform cameraPoint;

    #endregion

    #region Movement

        private bool rolled;
        public Rigidbody rb
        {
            get
            {
                if (_rb == null)
                    _rb = GetComponentInChildren<Rigidbody>();

                return _rb;
            }
        }
        private Rigidbody _rb;

    [SerializeField, FoldoutGroup("Dice"), Range(0f, 1f)]
    private float stickLaunchMagnitude = 0.99f;
    [SerializeField, FoldoutGroup("Dice"), Min(0f)]
    private float verticalLaunchForce = 5f, launchForce = 10f, rollSpeed = 500f, maxRollTime = 4f;

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
            cameraPoint.localPosition = cameraPointOffset;
            rollTime = 0f;

            enabledEvent.Invoke();

            rb.constraints = RigidbodyConstraints.FreezePosition;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;

            Vector2 rotationDirection = Vector2.zero;
            
            rb.transform.localPosition = Vector3.zero;
            rb.transform.localRotation = Quaternion.identity;

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

                cameraPoint.position = rb.transform.position + cameraPointOffset;

                yield return General.waitForFixedUpdate;
                rollTime += Time.fixedDeltaTime;
            }

            // Dice behavior when it's stopped rolling - find the side facing the camera.
            var upDir = dirs.OrderBy(x => Vector3.Angle(rb.transform.TransformDirection(x), Vector3.up)).First();

            thingValue = dirs.IndexOf(upDir) + 1;
            onValueChanged.Invoke(thingValue);

            Debug.Log($"Value: {thingValue}");
        }

        private void Launch(Vector2 direction)
        {
            if (rolled && thingValue <= 0)
                return;

            if (thingValue > 0)
                InitializeDice();

            rolled = true;

            side = dirs[Random.Range(0, dirs.Count)];

            direction.Normalize();

            rb.useGravity = true;

            rb.transform.Rotate(Random.Range(0, 4) * 90f, Random.Range(0, 4) * 90f, 0, Space.Self);

            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce((((Vector3.right * direction.x) + (Vector3.forward * direction.y)) * launchForce) + (Vector3.up * verticalLaunchForce), ForceMode.Impulse);
            rb.AddTorque(((Vector3.right * direction.y) + (Vector3.back * direction.x)) * rollSpeed, ForceMode.Impulse);
        }

    #endregion

    #region Actions

        void RandomLaunch()
        {
            Vector2 launchDirection = Random.insideUnitCircle.normalized;

            Launch(launchDirection);
        }

        public override void Move(Vector2 direction)
        {
            if (direction.magnitude >= stickLaunchMagnitude)
                Launch(direction);
        }

        public override void PrimaryAction(bool pressed)
        {
            RandomLaunch();
        }

        public override void SecondaryAction(bool pressed)
        {
            RandomLaunch();
        }

    #endregion
}

[System.Serializable]
public class DicePool
{
    [SerializeField] private GameObject dicePrefab;
    private List<Dice> dicePool = new List<Dice>();

    public Dice GetDieFromPool(Vector3 diePosition, UnityEngine.Events.UnityAction diceEnabledAction = null, UnityEngine.Events.UnityAction<int> diceValueChangeAction = null, UnityEngine.Events.UnityAction diceDisabledAction = null)
    {
        Dice die = null;

        foreach (Dice pooledDie in dicePool)
        {
            if (!pooledDie.gameObject.activeInHierarchy)
            {
                die = pooledDie;
                break;
            }
        }

        if (die == null)
        {
            die = GameObject.Instantiate(dicePrefab, diePosition, Quaternion.identity).GetComponent<Dice>();
            dicePool.Add(die);
        }


        die.enabledEvent.RemoveAllListeners();
        die.onValueChanged.RemoveAllListeners();
        die.disabledEvent.RemoveAllListeners();

        if (diceEnabledAction != null)
            die.enabledEvent.AddListener(diceEnabledAction);

        if (diceValueChangeAction != null)
            die.onValueChanged.AddListener(diceValueChangeAction);

        if (diceDisabledAction != null)
            die.disabledEvent.AddListener(diceDisabledAction);
            
        die.gameObject.SetActive(true);
        die.transform.position = diePosition;

        return die;
    }

    public void ReturnDieToPool(Dice dice)
    {
        dice.gameObject.SetActive(false);
    }
}
