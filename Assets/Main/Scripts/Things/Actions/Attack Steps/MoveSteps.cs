using System.Collections;
using UnityEngine;

public abstract class MovementControllerStep : AttackStep
{
    public AttackTarget moveTarget = AttackTarget.Target;
    protected bool moveConditionMet = false;

    protected Vector3 targetPosition = Vector3.zero;
    protected GameThing targetedThing = null;

    protected MovementController moveController = null;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        attacker.TryGetComponent(out moveController);

        moveController.canControl = 1;

        targetedThing = (targetThing != null) ? targetThing : attacker;

        switch (moveTarget)
        {
            case AttackTarget.Attacker:
                targetPosition = originalPosition;
                break;
            case AttackTarget.Target:
                targetPosition = target;
                break;
            case AttackTarget.TargetThing:
                targetPosition = targetedThing != null ? targetedThing.thingTop.position : originalPosition;
                break;
        }

        while (!moveConditionMet)
        {
            CheckMoveCondition();
            yield return null;
        }

        if (moveController != null)
        {
            moveController.canControl = 0;
            moveController.movementInput = Vector2.zero;
            moveController.jumpInput = false;

            moveController.canMove = false;
            moveController.canJump = false;
            moveController.jumpHeight = null;
        }

        callback?.Invoke(StepResult.Success);
    }

    public override void ResetStep()
    {
        moveConditionMet = false;
        targetPosition = Vector3.zero;
        targetedThing = null;

        if (moveController != null)
        {
            moveController.canControl = 0;
            moveController.movementInput = Vector2.zero;
            moveController.jumpInput = false;

            moveController = null;
        }
    }

    protected abstract void CheckMoveCondition();
}

public class MoveToTargetStep : MovementControllerStep
{
    public float stoppingDistance = 1f;

    protected override void CheckMoveCondition()
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
            Vector3 directionToTarget = (targetedThing.transform.position - moveController.transform.position).normalized;
            moveController.movementInput = new Vector2(directionToTarget.x, directionToTarget.z);
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

    protected override void CheckMoveCondition()
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
        Vector3 directionToTarget = (targetPosition - moveController.transform.position).normalized;
        moveController.movementInput = new Vector2(directionToTarget.x, directionToTarget.z);

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
