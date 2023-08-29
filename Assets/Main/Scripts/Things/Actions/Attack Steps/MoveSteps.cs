using System.Collections;
using UnityEngine;

public abstract class MovementControllerStep : AttackStep
{
    public AttackTarget moveTarget = AttackTarget.Target;
    protected bool moveConditionMet = false;

    protected Vector3 targetPosition = Vector3.zero;
    // protected GameThing targetedThing = null;

    protected MovementController moveController = null;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        attacker.TryGetComponent(out moveController);

        if (moveController == null)
        {
            callback?.Invoke(StepResult.Failure);
            yield break;
        }

        moveController.canControl = MovementController.ControlLevel.MovementOnly;

        attacker.TryGetComponent(out Collider collider);

        if (collider != null)
            collider.isTrigger = true;

        if (targetThing == null)
            targetThing = attacker;

        switch (moveTarget)
        {
            case AttackTarget.Attacker:
                targetPosition = originalPosition;
                break;
            case AttackTarget.Target:
                targetPosition = target;
                break;
            case AttackTarget.TargetThing:
                targetPosition = ((targetThing != null) ? targetThing.thingTop.position : originalPosition);
                break;
        }

        MovementController.MovementRotationBehavior originalRotationBehavior = moveController.rotationBehavior;

        if (targetPosition == originalPosition)
            moveController.rotationBehavior = MovementController.MovementRotationBehavior.None;

        Debug.Log($"Move to {(targetThing != null ? targetThing.thingName : targetPosition.ToString())} from {originalPosition}", attacker);

        while (!moveConditionMet)
        {
            CheckMoveCondition(targetThing);
            yield return null;
        }

        if (moveController != null)
        {
            moveController.canControl = MovementController.ControlLevel.None;
            moveController.movementInput = Vector2.zero;
            moveController.jumpInput = false;

            moveController.canMove = false;
            moveController.canJump = false;
            moveController.jumpHeight = null;

            moveController.rotationBehavior = originalRotationBehavior;
        }

        if (collider != null)
            collider.isTrigger = false;

        callback?.Invoke(StepResult.Success);
    }

    public override void ResetStep()
    {
        // Debug.Log("Resetting movement step");

        moveConditionMet = false;
        targetPosition = Vector3.zero;
        // targetedThing = null;

        if (moveController != null)
        {
            moveController.canControl = MovementController.ControlLevel.None;
            moveController.movementInput = Vector2.zero;
            moveController.jumpInput = false;

            moveController = null;
        }
    }

    protected abstract void CheckMoveCondition(GameThing targetedThing);
}

public class MoveToTargetStep : MovementControllerStep
{
    public float stoppingDistance = 1f;

    protected override void CheckMoveCondition(GameThing targetedThing)
    {
        if (moveController == null)
            return;

        moveController.canMove = true;

        // Condition for moving to target
        float distanceToTarget = Vector3.Distance(moveController.transform.position, targetedThing.transform.position);
        if (distanceToTarget <= stoppingDistance)
        {
            moveConditionMet = true;
        }
        else
        {
            // Move towards target
            Vector3 directionToTarget = (targetedThing.transform.position - moveController.transform.position);
            moveController.movementInput = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
        }
    }
}

public class JumpOnTargetStep : MovementControllerStep
{
    public float jumpDistance = 0.1f;
    private float? jumpHeight = null;
    private bool jumped = false;

    private bool checkGroundedCondition = false;

    public override void ResetStep()
    {
        base.ResetStep();

        checkGroundedCondition = false;
        jumpHeight = null;
        jumped = false;
    }

    protected override void CheckMoveCondition(GameThing targetedThing)
    {
        if (moveController == null)
            return;

        moveController.canMove = true;

        // Set jump parameters
        moveController.canJump = true;
        if (jumpHeight != null)
        {
            moveController.jumpHeight = targetPosition.y - moveController.transform.position.y;
            jumpHeight = moveController.jumpHeight;
        }
        moveController.jumpInput = true;

        // Move towards target
        Vector3 directionToTarget = (targetPosition - moveController.transform.position);
        moveController.movementInput = new Vector2(directionToTarget.x, directionToTarget.z).normalized;
        if (moveController.movementInput.magnitude < 0.1f)
            moveController.movementInput = Vector2.zero;

        // Condition for stopping movement
        float distanceToTarget = Vector3.Distance(moveController.transform.position, targetPosition);
        if (distanceToTarget <= jumpDistance || (checkGroundedCondition && moveController.IsGrounded() && jumped))
        {
            moveController.movementInput = Vector2.zero;
            moveController.jumpInput = false;
            jumpHeight = null;
            jumped = false; // Reset the flag as the jump has been completed and the character has landed
            moveConditionMet = true;
            checkGroundedCondition = false;
        }
        else if (jumped)
        {
            moveController.StartCoroutine(WaitToCheckGroundedCondition());
        }
        jumped = true; // Set the flag to true as a jump has been initiated
    }

    private IEnumerator WaitToCheckGroundedCondition()
    {
        yield return General.waitForFixedUpdate; // Wait for the next fixed update to check if the character has landed
        checkGroundedCondition = true;
    }
}
