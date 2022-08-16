using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Unity.Netcode;

[RequireComponent(typeof(PlayerInput))]
public class Player : NetworkBehaviour
{   
    #region Input variables

        [HideInInspector]
        public Thing controlledThing
        {
            get => _controlledThing;
            set => SetControlObject(value);
        }
        [SerializeField]
        private Thing _controlledThing;

        [HideInInspector]
        public Vector2 movement
        {
            get => _movement;
            set
            {
                _movement = value;

                moving = value.magnitude > 0;
            }
        }
        private Vector2 _movement;

        private bool moving;

        [HideInInspector]
        public bool button1
        {
            get => _button1;
            set
            {
                _button1 = value;

                if (value)
                {
                    if (button1)
                        controlledThing.PrimaryAction();
                }
            }
        }
        private bool _button1;
        
        public bool button2
        {
            get => _button2;
            set
            {
                _button2 = value;

                if (value)
                {
                    if (button2)
                        controlledThing.SecondaryAction();
                }
            }
        }
        private bool _button2;

        public bool canControl = true;

        private NetworkVariable<Vector2> _netMovement;
        private NetworkVariable<bool> _netButton1, _netButton2;

        public float inputAdjustTime = 0.05f;
        private float currentInputTime;

    #endregion

    #region Input functions

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            GameManager.AddPlayer(this);

            SetControlObject();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) Destroy(this);
        }

        public void OnMove(InputValue value)
        {
            movement = value.Get<Vector2>();
        }

        public void OnButton1(InputValue value)
        {
            button1 = value.isPressed;
        }

        public void OnButton2(InputValue value)
        {
            button2 = value.isPressed;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            GameManager.RemovePlayer(this);
        }

        public void Update()
        {
            if (IsOwner) TransmitState();
            else ConsumeState();

            if (canControl)
            {
                if (moving)
                {
                    currentInputTime = Mathf.Min(currentInputTime + Time.deltaTime, inputAdjustTime);

                    if (currentInputTime >= inputAdjustTime && controlledThing != null)
                    {
                        controlledThing.Move((Vector3.right * movement.x) + (Vector3.forward * movement.y));
                    }
                }
                else
                    currentInputTime = 0;
            }
        }

        private void TransmitState()
        {
            _netMovement.Value = movement;
            _netButton1.Value = button1;
            _netButton2.Value = button2;
        }

        private void ConsumeState()
        {
            movement = _netMovement.Value;
            button1 = _netButton1.Value;
            button2 = _netButton2.Value;
        }

    #endregion

    #region Control Object Functions

        public void SetControlObject(Thing thingToControl, bool immediateCameraShift = false)
        {
            if (_controlledThing != null)
                _controlledThing.useUI = false;

            if (thingToControl == null)
                if (_controlledThing != null)
                    thingToControl = _controlledThing;
                else
                    return;

            if (_controlledThing != thingToControl)
                _controlledThing = thingToControl;

            _controlledThing.useUI = true;

            GameplayCamera.SetCameraObject(_controlledThing, immediateCameraShift);

            Debug.Log("Set control object to " + thingToControl.name);
        }

        [Button]
        public void SetControlObject()
        {
            StartCoroutine(SettingControlObject());
        }

        IEnumerator SettingControlObject()
        {
            yield return null;

            SetControlObject(null);
        }

    #endregion
}
