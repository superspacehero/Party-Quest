using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Iris : MonoBehaviour
{
    [SerializeField] private Image irisImage;

    public enum IrisState
    {
        None = 0,
        Closing = -1,
        Opening = 1
    }

    [SerializeField] private IrisState startingState = IrisState.None;

    [SerializeField, Min(0.001f)] private float irisTime = 0.5f;
    [SerializeField, Min(0f)] private float irisWaitTime = 0.5f;

    public UnityEvent onOpen, onClose;

    private Material mat;

    private void Awake()
    {
        if (irisImage != null)
        {
            mat = Instantiate(irisImage.material);
            irisImage.material = mat;
        }
    }

    void OnEnable()
    {
        // Set the iris to the starting state
        switch ((int)startingState)
        {
            case <= 0:
                mat.SetFloat("_Circle_Size", 1f);
                break;
            case > 0:
                mat.SetFloat("_Circle_Size", 0f);
                break;
        }

        if (irisWaitTime > 0f)
            Invoke("AnimateIris", irisWaitTime);
        else
            AnimateIris();
    }

    [Button]
    public void AnimateIris(int state)
    {
        StopAllCoroutines();

        // Tween the iris open or closed
        switch (state)
        {
            case 0:
                // Debug.Log("Resetting iris");
                mat.SetFloat("_Circle_Size", 1f);
                irisImage.enabled = false;
                break;
            case < 0:
                // Debug.Log("Closing iris");
                StartCoroutine(AnimateIrisCoroutine(0f, irisTime, () => onClose.Invoke()));
                break;
            case > 0:
                // Debug.Log("Opening iris");
                StartCoroutine(AnimateIrisCoroutine(1f, irisTime, () => onOpen.Invoke()));
                break;
        }
    }

    private IEnumerator AnimateIrisCoroutine(float targetValue, float duration, UnityAction onComplete)
    {
        float startValue = mat.GetFloat("_Circle_Size");
        float elapsed = 0f;

        irisImage.enabled = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / duration);
            float currentValue = Mathf.Lerp(startValue, targetValue, normalizedTime);
            mat.SetFloat("_Circle_Size", currentValue);
            yield return null;
        }

        if (targetValue != 0f)
            irisImage.enabled = false;

        onComplete?.Invoke();
    }

    public void AnimateIris()
    {
        AnimateIris((int)startingState);
    }
}
