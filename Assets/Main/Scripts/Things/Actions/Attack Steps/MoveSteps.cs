using System.Collections;
using UnityEngine;

public abstract class MovementControllerStep : AttackStep
{
    protected bool moveConditionMet = false;

    protected GameThing targetThing = null;

    protected MovementController moveController = null;

    public override IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback)
    {
        attacker.TryGetComponent(out moveController);

        moveController.canControl = 1;

        targetThing = (target != null) ? target : attacker;

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
        targetThing = null;

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
        float distanceToTarget = Vector3.Distance(moveController.transform.position, targetThing.transform.position);
        if (distanceToTarget <= stoppingDistance)
        {
            moveConditionMet = true;
        }
        else
        {
            // Move towards target
            Vector3 directionToTarget = (targetThing.transform.position - moveController.transform.position).normalized;
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
            moveController.jumpHeight = targetThing.thingTop.position.y - moveController.transform.position.y;
            jumpHeight = moveController.jumpHeight;
        }
        moveController.jumpInput = true;

        // Move towards target
        Vector3 directionToTarget = (targetThing.thingTop.position - moveController.transform.position).normalized;
        moveController.movementInput = new Vector2(directionToTarget.x, directionToTarget.z);

        // Condition for stopping movement
        float distanceToTarget = Vector3.Distance(moveController.transform.position, targetThing.thingTop.position);
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
