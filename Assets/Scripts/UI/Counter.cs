using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class Counter : MonoBehaviour
{
    public int count
    {
        get
        {
            return _count;
        }
        set
        {
            _count = (!showZeroOrBelow && value < 0) ? 0 : value;

            Debug.Log("Count: " + _count);

            FlipNumber();
        }
    }
    private int _count = 0;

    [SerializeField]
    private bool showZeroOrBelow = false;

    #region Animation

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            FlipNumber();
        }

        TextMeshProUGUI text
        {
            get
            {
                if (_text == null)
                    TryGetComponent(out _text);

                return _text;
            }
        }
        TextMeshProUGUI _text;

        public float rotateTime = 0.25f;
        private float rotateTimeHalf
        {
            get { return rotateTime * 0.5f; }
        }
        public Vector3 firstRotation = new Vector3(0, -90, 0);
        public Vector3 lastRotation = new Vector3(0, 90, 0);
        public Vector3 rotationVariation = new Vector3(10, 0, 10);

        void FlipNumber()
        {
            StopCoroutine(Flipping());
            StartCoroutine(Flipping());
        }

        IEnumerator Flipping()
        {
            float t = 0;

            Quaternion _firstRotation = Quaternion.Euler(firstRotation + rotationVariation * Random.Range(-1f, 1f));
            Quaternion _lastRotation = Quaternion.Euler(lastRotation + rotationVariation * Random.Range(-1f, 1f));

            while (t < 1)
            {
                t += Time.deltaTime / rotateTimeHalf;
                transform.localRotation = Quaternion.Lerp(Quaternion.identity, _firstRotation, t);
                yield return null;
            }
            
            t = 0;
            SetText();

            while (t < 1)
            {
                t += Time.deltaTime / rotateTimeHalf;
                transform.localRotation = Quaternion.Lerp(_lastRotation, Quaternion.identity, t);
                yield return null;
            }
        }

        void SetText()
        {
            text.text = (showZeroOrBelow || (!showZeroOrBelow && count > 0)) ? count.ToString() : "";
        }

    #endregion

    #region Debug

        [Button]
        void IncreaseDigit()
        {
            count++;
        }

        [Button]
        void DecreaseDigit()
        {
            count--;
        }

    #endregion
}
