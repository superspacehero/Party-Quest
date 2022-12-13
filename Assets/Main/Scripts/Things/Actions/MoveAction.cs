using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : ActionThing
{
    // The number of spaces the character can move
    public int movementRange = 3;

    // The remaining number of spaces the character can move
    public int remainingMovement = 3;

    // The position-based version of the movement system for AI
    public Vector3 destination;

    protected override IEnumerator RunAction(GameThing user)
    {
        // Set the user's destination
        user.transform.position = destination;

        // While the user has remaining movement and hasn't reached their destination
        while (remainingMovement > 0 && user.transform.position != destination)
        {
            // Move the user towards their destination
            user.transform.position = Vector3.MoveTowards(user.transform.position, destination, Time.deltaTime);

            // Reduce the remaining movement by 1
            remainingMovement--;

            // Wait until the next frame
            yield return null;
        }

        // Reset the remaining movement
        remainingMovement = movementRange;

        // The action is no longer running
        actionRunning = false;
    }
}