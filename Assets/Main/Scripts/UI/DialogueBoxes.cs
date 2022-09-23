using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBoxes : MonoBehaviour
{
    [SerializeField] private General.RectTransformAnchorPreset upperPreset, middlePreset, lowerPreset;
    [SerializeField] private DialogueBox speechBubble, dialogueBox;

    public void EnableSpeechBubble(string text, Transform target, bool isSpeech = true)
    {
        ((isSpeech) ? speechBubble : dialogueBox).Enable(text, target);
    }
}
