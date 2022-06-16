using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamera : InstancedObject
{
    #region Camera variables

        new protected static GameplayCamera instance;
        public float cameraAdjustTime = 0.25f;

    #endregion



    #region Camera functions

        public static void SetCameraObject(Thing thingToFollow, bool immediateCameraShift = false)
        {
            instance.transform.SetParent(thingToFollow.cameraPoint, true);

            if (immediateCameraShift)
                instance.transform.localPosition = Vector3.zero;
            else
                instance.StartCoroutine(CenterCamera());
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
}
