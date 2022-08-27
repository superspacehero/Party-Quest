using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBrush : Thing
{
    [SerializeField] private Tilemap currentTilemap;
    [SerializeField] private LevelTile currentTile;

    private enum BrushInput
    {
        None,
        Brush,
        Erase
    }

    private BrushInput brushInput;

    public override void PrimaryAction(bool runningAction)
    {
        if (runningAction)
            brushInput = BrushInput.Brush;
        else if (brushInput == BrushInput.Brush)
            brushInput = BrushInput.None;

        Debug.Log(brushInput);
    }

    public override void SecondaryAction(bool runningAction)
    {
        if (runningAction)
            brushInput = BrushInput.Erase;
        else if (brushInput == BrushInput.Erase)
            brushInput = BrushInput.None;

        Debug.Log(brushInput);
    }

    void PlaceTile(Vector3 position)
    {
        currentTilemap.SetTile(currentTilemap.WorldToCell(position), currentTile);
    }

    void RemoveTile(Vector3 position)
    {
        currentTilemap.SetTile(currentTilemap.WorldToCell(position), null);
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        switch (brushInput)
        {
            case BrushInput.Brush:
                PlaceTile(transform.position);
                break;
            case BrushInput.Erase:
                RemoveTile(transform.position);
                break;
        }
    }
}
