using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
                instance.CenterCamera();

            Debug.Log("Set follow object to " + thingToFollow.name);
        }

        public void RotateCamera(float rotation)
        {
            cameraRotation.x = rotation;
        }

        public void RotateCamera(Vector3 rotation)
        {
            cameraRotation = rotation;
        }

        public void RotateCamera(Vector4 rotationTime)
        {
            cameraRotation = new Vector3(rotationTime.x, rotationTime.y, rotationTime.z);
            CenterCamera(rotationTime.w);
        }

        public void CenterCamera(float centerTime = -1f)
        {
            centeringCamera = true;
            transform.DOLocalMove(cameraOffset, centerTime < 0 ? cameraAdjustTime : centerTime);
            transform.DOLocalRotate(cameraRotation, centerTime < 0 ? cameraAdjustTime : centerTime).onComplete = () => { centeringCamera = false; };
        }

        private bool centeringCamera;

    #endregion
}
