using UnityEngine;
using UnityEngine.EventSystems;

////TODO: custom icon for OnScreenButton component

/// <summary>
/// A button that is visually represented on-screen and triggered by touch or other pointer
/// input.
/// </summary>
[AddComponentMenu("Input/On-Screen Button - Enum")]
public class OnScreenButtonEnum : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Primary,
        Secondary,
        Pause
    }

    public ButtonType buttonType;

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Primary:
                GameManager.currentCharacter?.PrimaryAction(true);
                break;
            case ButtonType.Secondary:
                GameManager.currentCharacter?.SecondaryAction(true);
                break;
            case ButtonType.Pause:
                GameManager.currentCharacter?.Pause();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (buttonType)
        {
            case ButtonType.Primary:
                GameManager.currentCharacter?.PrimaryAction(false);
                break;
            case ButtonType.Secondary:
                GameManager.currentCharacter?.SecondaryAction(false);
                break;
        }
    }
}