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

        public static Thing cameraObject;

    #endregion



    #region Camera functions

        public static void SetCameraObject(MovingThing thingToFollow, bool immediateCameraShift = false)
        {
            if (thingToFollow.controlledThing != null && thingToFollow.controlledThing.moveCameraToMeWhenControlling)
                thingToFollow = thingToFollow.controlledThing;

            if (!thingToFollow.moveCameraToMeWhenControlling)
                return;

            cameraObject = thingToFollow;

            instance.transform.SetParent(thingToFollow.cameraPoint, true);

            if (immediateCameraShift)
                instance.transform.localPosition = Vector3.zero;
            else
                instance.StartCoroutine(instance.CenterCamera());

            Debug.Log("Set follow object to " + thingToFollow.name);
        }

        public IEnumerator CenterCamera()
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

    #region Rotating Things

        private float previousRotation;

        public static List<RotateTowardsCamera> cameraRotators = new List<RotateTowardsCamera>();

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // if (transform.hasChanged)
            if (previousRotation != transform.eulerAngles.y)
                foreach (RotateTowardsCamera rotator in cameraRotators)
                    rotator.Rotate(myCamera);

            previousRotation = transform.eulerAngles.y;
        }

    #endregion
}
