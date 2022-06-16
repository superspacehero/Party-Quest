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

        public bool canControl;

        public float inputAdjustTime = 0.05f;
        private float currentInputTime;

        bool moving, button1, button2;

    #endregion

    #region Input functions

        public void OnMove(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            input.x = Mathf.Abs(input.x);
            input.y = Mathf.Abs(input.y);

            if (input.magnitude > 0 && input.x != input.y)
            {
                if (input.x > input.y)
                    input.y = 0;
                else if (input.y > input.x)
                    input.x = 0;
            }

            movement = Vector2Int.RoundToInt(input);

            moving = movement.magnitude > 0;
        }

        public void OnButton1(InputValue value)
        {
            button1 = value.isPressed;
        }

        public void OnButton2(InputValue value)
        {
            button2 = value.isPressed;
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
    public void SetControlObject(Thing thingToControl, bool immediateCameraShift = false)
    {
        if (thingToControl == null)
            return;

        if (controlledThing != thingToControl)
            _controlledThing = thingToControl;

        GameplayCamera.SetCameraObject(controlledThing, immediateCameraShift);

        Debug.Log("Set control object to " + thingToControl.name);
    }
}
