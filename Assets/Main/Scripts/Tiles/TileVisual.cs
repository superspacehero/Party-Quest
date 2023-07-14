using UnityEngine;

public class TileVisual : MonoBehaviour
{
    public GameObject tileVisuals;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected void Awake()
    {
        if (GameManager.instance && GameManager.gameMode != GameMode.Make)
            tileVisuals?.SetActive(false);
    }
}