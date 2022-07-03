using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;

public class Thing : LookAtObject
{
    // This class is used as the basis for all objects in the game.
    // It contains the basic properties of an object, such as its localized name,
    // its localized description, its icon, and its value.

    [FoldoutGroup("Info")]
    public LocalizedString nameString;
    [FoldoutGroup("Info")]
    public LocalizedString descriptionString;
    [FoldoutGroup("Info")]
    public Sprite icon;
    [FoldoutGroup("Info")]
    public int value;

    #region Health

        public Health health;

    #endregion

    #region Camera/UI

        public Transform cameraPoint
        {
            get
            {
                if (_cameraPoint == null)
                    _cameraPoint = transform;

                return _cameraPoint;
            }
        }
        [SerializeField]
        private Transform _cameraPoint;

        public bool moveCameraToMeWhenControlling;

        [ReadOnly]
        public bool useUI = false;

    #endregion

    #region Movement

        protected bool moving;
        public float tileMoveTime = 0.2f, maxStepHeight = 0.5f;

        protected Vector3 direction;

        // Move to a new node.
        public virtual void MoveTo(Vector3 position, bool ignoreCollisions = false, bool checkHeight = true)
        {
            direction = position - transform.position;
            transform.position = position;
        }

        public virtual void Move(Vector3 direction, bool ignoreCollisions = false, bool checkHeight = true)
        {
            MoveTo(transform.position + direction);
        }

    #endregion

    #region Rotation

        public enum MovementRotationBehavior
        {
            None,
            FullRotation,
            LeftRightRotation
        }
        [SerializeField, FoldoutGroup("Rotation")]
        private MovementRotationBehavior rotationBehavior = MovementRotationBehavior.None;

        protected Transform meshBase
        {
            get
            {
                if (_meshBase == null)
                    _meshBase = transform;

                return _meshBase;
            }
        }
        [SerializeField, InfoBox("If left empty, the base object will be used."), FoldoutGroup("Rotation")]
        private Transform _meshBase;

        public float gotoRotation
        {
            get { return _gotoRotation; }

            set
            {
                if (_gotoRotation != value)
                    RotateTowardsGotoRotation();
                
                _gotoRotation = value;
            }
        }
        // [SerializeField, ReadOnly, FoldoutGroup("Rotation")]
        private float _gotoRotation;

        [FoldoutGroup("Rotation")]
        public float rotationTime = 0.25f;

        protected Vector3 rotationDirection = Vector3.right;

        public void Rotate(Vector3 direction)
        {
            switch (rotationBehavior)
            {
                case MovementRotationBehavior.None:
                    return;
                case MovementRotationBehavior.FullRotation:
                    break;
                case MovementRotationBehavior.LeftRightRotation:
                    direction.z *= 0.5f;
                    if (direction.x == 0)
                        direction.x = rotationDirection.x;
                    break;
            }

            rotationDirection = direction;
            gotoRotation = ((Mathf.Atan2(rotationDirection.x, rotationDirection.z)) * Mathf.Rad2Deg) + (rotationBehavior == MovementRotationBehavior.LeftRightRotation ? -90 : 0);
        }

        [Button, FoldoutGroup("Rotation"), HideInEditorMode]
        public void RandomRotation()
        {
            gotoRotation = Random.Range(0f, 360f);
        }

        private void RotateTowardsGotoRotation()
        {
            StopCoroutine(Rotating());
            StartCoroutine(Rotating());
        }

        private IEnumerator Rotating()
        {
            float startTime = 0;
            // rotating = true;

            while (startTime < 1)
            {
                _meshBase.localEulerAngles = new Vector3(_meshBase.localEulerAngles.x, Mathf.LerpAngle(_meshBase.localEulerAngles.y, gotoRotation, rotationTime), _meshBase.localEulerAngles.z);
                startTime += Time.deltaTime / rotationTime;
                yield return null;
            }

            _meshBase.localEulerAngles = new Vector3(
                _meshBase.localEulerAngles.x,
                gotoRotation,
                _meshBase.localEulerAngles.z
            );

            // rotating = false;
        }

    #endregion

    #region Actions

        public virtual void PrimaryAction()
        {
            
        }

        public virtual void SecondaryAction()
        {
            
        }

    #endregion

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw an arrow pointing in the direction of, well, direction.
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, direction * 1.5f);
    }
}
