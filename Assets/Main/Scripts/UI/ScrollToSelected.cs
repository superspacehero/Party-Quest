using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollToSelected : MonoBehaviour
{
    private ScrollRect scrollRect
    {
        get
        {
            if (_scrollRect == null)
                TryGetComponent(out _scrollRect);

            return _scrollRect;
        }
    }
    [SerializeField] private ScrollRect _scrollRect;

    [SerializeField] private float scrollTime = 0.2f;


    public GameObject selectedObject;

    private void Update()
    {
        if (selectedObject != null)
        {
            scrollRect.content.DOAnchorPos(
                (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(selectedObject.transform.position),
                scrollTime).OnUpdate(() => Debug.Log("Scrolling..."));

            selectedObject = null;
        }
    }

    public void UpdateSelectedObject(GameObject newSelectedObject)
    {
        selectedObject = newSelectedObject;
    }
}
