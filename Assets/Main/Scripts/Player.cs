using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{   
    #region Input variables

        [HideInInspector]
        public Thing controlObject;

        [HideInInspector]
        public Vector2 input;

        [HideInInspector]
        public bool updating;

        public bool canControl;

        public float inputAdjustTime = 0.05f;
        float currentInputTime;

        bool moving, button1, button2;

    #endregion

    #region Camera variables

        public float cameraAdjustTime = 0.25f;

    #endregion

    #region Input functions

        public void OnMove(InputValue value)
        {
            input = Vector2Int.RoundToInt(value.Get<Vector2>().normalized);

            moving = input.magnitude > 0;
        }

        public void OnButton1(InputValue value)
        {
            button1 = value.isPressed;
        }

        public void OnButton2(InputValue value)
        {
            button2 = value.isPressed;
        }

    #endregion

    #region Camera functions

        public void SetControlObject(Thing thingToControl, bool immediateCameraShift = false)
        {
            controlObject = thingToControl;

            SetCameraObject(controlObject, immediateCameraShift);
        }

        public void SetCameraObject(Thing thingToFollow, bool immediateCameraShift = false)
        {
            transform.SetParent(thingToFollow.cameraPoint, true);

            if (immediateCameraShift)
                transform.localPosition = Vector3.zero;
            else
                StartCoroutine(CenterCamera());
        }

        IEnumerator CenterCamera()
        {
            float cameraProgress = 0;
            Vector3 originalPosition = transform.localPosition;
            Quaternion originalRotation = transform.localRotation;

            while (cameraProgress <= 1)
            {
                cameraProgress += Time.deltaTime / cameraAdjustTime;
                transform.localPosition = Vector3.Slerp(originalPosition, Vector3.zero, cameraProgress);
                transform.localRotation = Quaternion.Slerp(originalRotation, Quaternion.identity, cameraProgress);

                yield return null;
            }

            transform.localPosition = Vector3.zero;
        }

    #endregion
}
