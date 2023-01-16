using UnityEngine;
using UnityEngine.UI;

public class ScrollToSelected : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject selectedObject;

    private void Update()
    {
        if (selectedObject != null)
        {
            var viewportPosition = scrollRect.viewport.InverseTransformPoint(selectedObject.transform.position);
            var viewportHeight = scrollRect.viewport.rect.height;
            var contentHeight = scrollRect.content.rect.height;
            var scrollPosition = scrollRect.verticalNormalizedPosition;

            if (viewportPosition.y > viewportHeight / 2)
            {
                scrollPosition -= (viewportPosition.y - viewportHeight / 2) / contentHeight;
            }
            else if (viewportPosition.y < -viewportHeight / 2)
            {
                scrollPosition -= (viewportPosition.y + viewportHeight / 2) / contentHeight;
            }

            scrollRect.verticalNormalizedPosition = scrollPosition;

            selectedObject = null;
        }
    }

    public void UpdateSelectedObject(GameObject newSelectedObject)
    {
        selectedObject = newSelectedObject;
    }
}
