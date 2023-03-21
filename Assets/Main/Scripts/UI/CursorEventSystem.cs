using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorEventSystem : EventSystem
{
    [SerializeField] private RectTransform selectionCursor;
    [SerializeField] private float cursorMoveTime = 0.1f;

    new private GameObject lastSelectedGameObject;

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected override void Update()
    {
        base.Update();

        if (currentSelectedGameObject != lastSelectedGameObject)
            SetCursor();

        lastSelectedGameObject = currentSelectedGameObject;
    }

    private void SetCursor()
    {
        if (currentSelectedGameObject != null)
        {
            if (currentSelectedGameObject.TryGetComponent(out RectTransform rectTransform))
            {
                StartCoroutine(MoveCursor(rectTransform));
            }
        }
        else
        {
            selectionCursor.gameObject.SetActive(false);
            lastSelectedGameObject = null;
        }

    }

    private float elapsedTime = 0f;
    private Vector2 originalAnchorMin, originalAnchorMax, originalOffsetMin, originalOffsetMax;

    private IEnumerator MoveCursor(RectTransform rectTransform)
    {
        selectionCursor.gameObject.SetActive(true);
        selectionCursor.SetParent(rectTransform, true);
        selectionCursor.localScale = Vector3.one;

        elapsedTime = 0f;

        originalAnchorMin = selectionCursor.anchorMin;
        originalAnchorMax = selectionCursor.anchorMax;
        originalOffsetMin = selectionCursor.offsetMin;
        originalOffsetMax = selectionCursor.offsetMax;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.fixedDeltaTime / cursorMoveTime;

            selectionCursor.anchorMin = Vector2.Lerp(originalAnchorMin, Vector2.zero, elapsedTime);
            selectionCursor.anchorMax = Vector2.Lerp(originalAnchorMax, Vector2.one, elapsedTime);
            selectionCursor.offsetMin = Vector2.Lerp(originalOffsetMin, Vector2.zero, elapsedTime);
            selectionCursor.offsetMax = Vector2.Lerp(originalOffsetMax, Vector2.zero, elapsedTime);

            yield return General.waitForFixedUpdate;
        }

        selectionCursor.anchorMin = Vector2.zero;
        selectionCursor.anchorMax = Vector2.one;
        selectionCursor.offsetMin = Vector2.zero;
        selectionCursor.offsetMax = Vector2.zero;
    }

    [UnityEditor.MenuItem("Tools/Replace Event System")]
    private static void ReplaceEventSystem()
    {
        var eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            DestroyImmediate(eventSystem);
            var cursorEventSystem = eventSystem.gameObject.AddComponent<CursorEventSystem>();

            // Add an undo action to the undo stack
            UnityEditor.Undo.RegisterCreatedObjectUndo(cursorEventSystem.gameObject, "Replace Event System");
        }
        else
        {
            Debug.LogError("No EventSystem found in scene");
        }
    }
}
