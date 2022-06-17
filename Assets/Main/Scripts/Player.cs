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
        public Vector2Int movement;

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
            Vector2 input = value.Get<Vector2>();

            movement = Vector2Int.RoundToInt(input);

            moving = movement.magnitude > 0;

            // Debug.Log(movement);
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
            SetControlObject();
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

    [Button]
    public void SetControlObject(Thing thingToControl = null, bool immediateCameraShift = false)
    {
        if (thingToControl == null)
            if (_controlledThing != null)
                thingToControl = _controlledThing;
            else
                return;

        if (controlledThing != thingToControl)
            _controlledThing = thingToControl;

        GameplayCamera.SetCameraObject(controlledThing, immediateCameraShift);

        Debug.Log("Set control object to " + thingToControl.name);
    }
}
