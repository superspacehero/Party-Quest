using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{   
    #region Input variables

        [HideInInspector]
        public Thing controlledThing
        {
            get => _controlledThing;
            set
            {
                SetControlObject(value);
            }
        }
        [SerializeField]
        private Thing _controlledThing;

        [HideInInspector]
        public Vector2 movement;

        [HideInInspector]
        public bool updating;

        public bool canControl = true;

        public float inputAdjustTime = 0.05f;
        private float currentInputTime;

        bool moving, button1, button2;

    #endregion

    #region Input functions

        public void OnMove(InputValue value)
        {
            movement = value.Get<Vector2>();

            moving = movement.magnitude > 0;
        }

        public void OnButton1(InputValue value)
        {
            button1 = value.isPressed;

            if (button1)
                controlledThing.PrimaryAction();
        }

        public void OnButton2(InputValue value)
        {
            button2 = value.isPressed;

            if (button2)
                controlledThing.SecondaryAction();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            GameManager.AddPlayer(this);

            SetControlObject();
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

    #endregion

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
}
