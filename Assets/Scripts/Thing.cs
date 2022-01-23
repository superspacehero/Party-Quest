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

    #region Rotation

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
                _gotoRotation = value;
                RotateTowardsGotoRotation();
            }
        }
        [SerializeField, ReadOnly, FoldoutGroup("Rotation")]
        private float _gotoRotation;

        [FoldoutGroup("Rotation")]
        public float rotationTime = 0.25f;

        private bool rotating = false;

        public void Rotate(Vector3 position, bool relative = true)
        {
            gotoRotation = (((relative) ? Mathf.Atan2(position.x, position.z) : Mathf.Atan2(position.x - transform.position.x, position.z - transform.position.z)) * Mathf.Rad2Deg);
        }

        [Button, FoldoutGroup("Rotation")]
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
            rotating = true;

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

            rotating = false;
        }

    #endregion
}
