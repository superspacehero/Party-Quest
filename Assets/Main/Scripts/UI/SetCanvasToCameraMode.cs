using UnityEngine;

public class SetCanvasToCameraMode : MonoBehaviour
{
    private void OnEnable()
    {
        if (TryGetComponent(out Canvas canvas))
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = GameplayCamera.instance.myCamera;
        }
    }
}