using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    #region Camera variables

        public static GameplayCamera instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameplayCamera>();

                return _instance;
            }
        }
        private static GameplayCamera _instance;

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            if (_instance == this)
                _instance = null;
        }

        public float cameraAdjustTime = 0.25f;
        private Camera myCamera
        {
            get
            {
                if (_myCamera == null)
                    TryGetComponent(out _myCamera);

                return _myCamera;
            }
        }
        private Camera _myCamera;

        public static GameThing cameraObject;

        public Vector3 cameraOffset = new Vector3(0f, 3.5f, -7f), cameraRotation = new Vector3(22.5f, 0f, 0f);

    #endregion



    #region Camera functions

        public static void SetCameraObject(GameThing thingToFollow, bool immediateCameraShift = false)
        {
            if (instance.centeringCamera && thingToFollow == cameraObject)
                return;

            if (thingToFollow == null)
            {
                Debug.LogError("Tried to set camera object to null!");
                return;
            }

            if (thingToFollow.GetAttachedThing() != null)
            {
                SetCameraObject(thingToFollow.GetAttachedThing(), immediateCameraShift);
                return;
            }

            cameraObject = thingToFollow;

            instance.transform.SetParent(thingToFollow.transform, true);

            if (immediateCameraShift)
            {
                instance.transform.localPosition = instance.cameraOffset;
                instance.transform.localEulerAngles = instance.cameraRotation;
            }
            else
                instance.StartCoroutine(instance.CenterCamera());

            Debug.Log("Set follow object to " + thingToFollow.name);
        }

        private bool centeringCamera;

        public IEnumerator CenterCamera()
        {
            centeringCamera = true;
            float cameraProgress = 0;
            Vector3 originalPosition = transform.localPosition;
            Vector3 originalRotation = transform.localEulerAngles;

            while (cameraProgress <= 1)
            {
                cameraProgress += Time.deltaTime / cameraAdjustTime;
                transform.localPosition = Vector3.Slerp(originalPosition, cameraOffset, cameraProgress);
                transform.localEulerAngles = Vector3.Slerp(originalRotation, cameraRotation, cameraProgress);

                yield return null;
            }

            transform.localPosition = cameraOffset;
            transform.localEulerAngles = cameraRotation;
            centeringCamera = false;
        }

    #endregion
}
