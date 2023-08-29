using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LevelEditorAction : ActionThing
{
    private LevelTile currentTile
    {
        get
        {
            if (_currentTile == null)
                _currentTile = TilemapManager.instance.tileTypeList.tileTypes[0];
            return _currentTile;
        }
        set => _currentTile = value;
    }
    private LevelTile _currentTile;

    private GameObject currentObject
    {
        get
        {
            if (_currentObject == null)
                _currentObject = TilemapManager.instance.gameObjectList.categories[0].gameObjects[0];
            return _currentObject;
        }
        set => _currentObject = value;
    }
    private GameObject _currentObject;

    private enum BrushInput
    {
        None,
        Brush,
        Erase
    }

    private BrushInput brushInput;

    public override void PrimaryAction(bool pressed)
    {
        if (pressed)
        {
            previousPosition = null;
            brushInput = BrushInput.Brush;
        }
        else if (brushInput == BrushInput.Brush)
            brushInput = BrushInput.None;
    }

    public override void SecondaryAction(bool pressed)
    {
        if (pressed)
        {
            previousPosition = null;
            brushInput = BrushInput.Erase;
        }
        else if (brushInput == BrushInput.Erase)
            brushInput = BrushInput.None;
    }

    public override void TertiaryAction(bool pressed)
    {
        if (pressed && user is CharacterThing)
        {
            previousPosition = null;

            Vector3 targetPosition = new Vector3(
                user.transform.position.x,
                Mathf.RoundToInt(user.transform.position.y) + 1,
                user.transform.position.z);

            (user as CharacterThing).movementController.rb.MovePosition(targetPosition);
        }
    }

    public override void QuaternaryAction(bool pressed)
    {
        int roundedY = Mathf.RoundToInt(user.transform.position.y);

        if (pressed && user is CharacterThing && roundedY >= 1)
        {
            previousPosition = null;

            Vector3 targetPosition = new Vector3(
                user.transform.position.x,
                roundedY - 1,
                user.transform.position.z);

            (user as CharacterThing).movementController.rb.MovePosition(targetPosition);
        }
    }

    [SerializeField] private float holdTime = 0.5f;
    private float pressTime = 0, releaseTime = 0;

    // Whether we are in Object mode or Tile mode
    private EditMode editMode = EditMode.Tile;

    public enum EditMode
    {
        Tile,
        Object
    }

    public override void LeftAction(bool pressed)
    {
        if (pressed)
        {
            pressTime = Time.time;
        }
        else
        {
            releaseTime = Time.time;

            if (releaseTime - pressTime < holdTime)
            {
                EditMode previousEditMode = editMode;
                editMode = EditMode.Tile;
                if (editMode != previousEditMode)
                    return;

                previousPosition = null;

                if (currentTile != null)
                {
                    int index = TilemapManager.instance.tileTypeList.tileTypes.IndexOf(currentTile);

                    if (index == TilemapManager.instance.tileTypeList.tileTypes.Count - 1)
                        currentTile = TilemapManager.instance.tileTypeList.tileTypes[0];
                    else
                        currentTile = TilemapManager.instance.tileTypeList.tileTypes[index + 1];

                    Debug.Log($"Current tile: {currentTile.name}");
                }
            }
            else
            {
                // Open tile selection menu
                LevelEditor.instance?.tileListMenu?.Select();
            }
        }
    }

    public override void RightAction(bool pressed)
    {
        if (pressed)
        {
            pressTime = Time.time;
        }
        else
        {
            releaseTime = Time.time;

            if (releaseTime - pressTime < holdTime)
            {
                EditMode previousEditMode = editMode;
                editMode = EditMode.Object;
                if (editMode != previousEditMode)
                    return;

                previousPosition = null;

                if (TilemapManager.instance.gameObjectList.Find(currentObject, out Vector2Int categoryAndObjectIndex))
                {
                    // If the current object is the last object in its category, go to the first object in the next category.
                    // If the category is the last category, go to the first object in the first category.
                    if (categoryAndObjectIndex.y == TilemapManager.instance.gameObjectList.categories[categoryAndObjectIndex.x].gameObjects.Length - 1)
                    {
                        if (categoryAndObjectIndex.x == TilemapManager.instance.gameObjectList.categories.Length - 1)
                            currentObject = TilemapManager.instance.gameObjectList.categories[0].gameObjects[0];
                        else
                            currentObject = TilemapManager.instance.gameObjectList.categories[categoryAndObjectIndex.x + 1].gameObjects[0];
                    }
                    else
                        currentObject = TilemapManager.instance.gameObjectList.categories[categoryAndObjectIndex.x].gameObjects[categoryAndObjectIndex.y + 1];
                }
                else
                    currentObject = TilemapManager.instance.gameObjectList.categories[0].gameObjects[0];

                Debug.Log($"Current object: {currentObject.name}");
            }
            else
            {
                // Open object selection menu
                LevelEditor.instance?.objectListMenu?.Select();
            }
        }
    }

    void PlaceTile(Vector3 position, LevelTile tile)
    {
        Vector3Int cellPosition = TilemapManager.instance.tilemap.WorldToCell(position);

        // Iteratively set cellPosition.z from the negated height of the tilemap to the height of the tilemap
        for (int i = TilemapManager.instance.tilemap.origin.z; i < TilemapManager.instance.tilemap.origin.z + TilemapManager.instance.tilemap.size.z; i++)
        {
            cellPosition.z = i;
            TilemapManager.instance.tilemap.SetTile(cellPosition, null);
        }

        cellPosition.z = Mathf.RoundToInt(position.y);
        TilemapManager.instance.tilemap.SetTile(cellPosition, tile);
    }

    LevelTile GetTile(Vector3 position)
    {
        Vector3Int cellPosition = TilemapManager.instance.tilemap.WorldToCell(position);
        cellPosition.z = Mathf.RoundToInt(position.y);

        return TilemapManager.instance.tilemap.GetTile(cellPosition) as LevelTile;
    }

    void RemoveTile(Vector3 position)
    {
        // Remove the tile at the current position
        PlaceTile(position, null);
    }

    Vector3? previousPosition = null;
    protected override IEnumerator RunAction()
    {
        // Set up the player to move properly

        if (user is CharacterThing)
            (user as CharacterThing).input.canControl = true;

        user.TryGetComponent(out MovementController controller);
        if (controller != null)
        {
            controller.canControl = MovementController.ControlLevel.Full;

            controller.canMove = true;

            controller.gravity = 0;
            controller.currentVerticalSpeed = 0;
        }

        user.TryGetComponent(out Collider collider);

        if (collider != null)
            collider.isTrigger = true;

        // While the user is still moving and hasn't stopped their movement turn
        while (actionRunning)
        {
            if (brushInput != BrushInput.None && (previousPosition == null || previousPosition != user.transform.position))
            {
                switch (brushInput)
                {
                    case BrushInput.Brush:
                        switch (editMode)
                        {
                            case EditMode.Tile:
                                PlaceTile(user.transform.position, currentTile);
                                break;
                            case EditMode.Object:
                                TilemapManager.instance.AddObjectToMap(currentObject, user.transform.position);
                                break;
                        }
                        break;
                    case BrushInput.Erase:
                        switch (editMode)
                        {
                            case EditMode.Tile:
                                RemoveTile(user.transform.position);
                                break;
                            case EditMode.Object:
                                TilemapManager.instance.RemoveObjectFromMap(user.transform.position);
                                break;
                        }
                        break;

                }

                previousPosition = user.transform.position;
            }

            // Wait until the next frame
            yield return General.waitForFixedUpdate;
        }

        // Disable movement control
        if (controller != null)
            controller.canControl = MovementController.ControlLevel.None;

        if (collider != null)
            collider.isTrigger = false;

        // The action is no longer running
        EndAction();
    }
}
