using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : ActionThing
{
// MoveAction is a subclass of ActionThing that handles moving a GameThing from one grid location to another.
// It takes a start position, an end position, and a duration, and moves the target GameThing from the start position to the end position over the given duration.
// It also checks if the target GameThing is a GridThing and, if so, updates its grid position to the end position on completion.

private Vector3 startPosition;
private Vector3 endPosition;
private float duration;

public override IEnumerator DoAction()
{
    // Set the start position to the current position of the target GameThing.
    startPosition = targetThing.transform.position;

    // Calculate the end position by converting the end grid position to world space.
    endPosition = GridThing.GridToWorld(endGridPosition);

    // Calculate the duration by dividing the distance between the start and end positions by the speed.
    duration = Vector3.Distance(startPosition, endPosition) / speed;

    // Set the start time to the current time.
    float startTime = Time.time;

    // While the elapsed time is less than the duration...
    while (Time.time - startTime < duration)
    {
        // Calculate the progress as a value between 0 and 1, representing how far along the move is.
        float progress = (Time.time - startTime) / duration;

        // Set the target GameThing's position to the interpolated value between the start and end positions.
        targetThing.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

        // Wait for the next frame.
        yield return null;
    }

    // Set the target GameThing's position to the end position.
    targetThing.transform.position = endPosition;

    // If the target GameThing is a GridThing, update its grid position to the end position.
    if (targetThing is GridThing gridThing)
    {
        gridThing.gridPosition = endGridPosition;
    }

    // Set the action as completed.
    isCompleted = true;
}

    // We need a constructor for MoveAction that takes a target GameThing, an end grid position, and a speed.
    public MoveAction(GameThing targetThing, Vector2Int endGridPosition, float speed)
    {
        this.targetThing = targetThing;
        this.endGridPosition = endGridPosition;
        this.speed = speed;
    }
}