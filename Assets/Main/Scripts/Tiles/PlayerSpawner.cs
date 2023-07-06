using UnityEngine;

public class PlayerSpawner : GameThing
{
    public override string thingType => "PlayerSpawner";

    public GameObject spawnerVisuals;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Awake()
    {
        if (GameManager.instance && GameManager.gameMode != GameMode.Make)
            spawnerVisuals?.SetActive(false);
    }
}