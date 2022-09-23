using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform speechBubblePoint;

    public void Enable(string text, Transform target)
    {
        dialogueText.text = text;

        if (speechBubblePoint != null)
        {
            if (target != null)
                speechBubblePoint.position = target.position;

            speechBubblePoint.gameObject.SetActive(target != null);
        }

        gameObject.SetActive(true);
    }
}
