using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamera : InstancedObject
{
    #region Camera variables

        new public static GameplayCamera instance;
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

    #endregion



    #region Camera functions

        public static void SetCameraObject(Thing thingToFollow, bool immediateCameraShift = false)
        {
            // instance.transform.SetParent(thingToFollow.cameraPoint, true);

            // if (immediateCameraShift)
            //     instance.transform.localPosition = Vector3.zero;
            // else
            //     instance.StartCoroutine(CenterCamera());
        }

        public static IEnumerator CenterCamera()
        {
            float cameraProgress = 0;
            Vector3 originalPosition = instance.transform.localPosition;
            Quaternion originalRotation = instance.transform.localRotation;

            while (cameraProgress <= 1)
            {
                cameraProgress += Time.deltaTime / instance.cameraAdjustTime;
                instance.transform.localPosition = Vector3.Slerp(originalPosition, Vector3.zero, cameraProgress);
                instance.transform.localRotation = Quaternion.Slerp(originalRotation, Quaternion.identity, cameraProgress);

                yield return null;
            }

            instance.transform.localPosition = Vector3.zero;
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
