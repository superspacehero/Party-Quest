using System.Collections;
using UnityEngine;

public abstract class WaitForStep : AttackStep
{
    protected bool waitConditionMet = false;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetObject = null)
    {
        while (!waitConditionMet)
        {
            CheckWaitCondition();
            yield return null;
        }

        callback?.Invoke(StepResult.Success);
    }

    public override void ResetStep()
    {
        waitConditionMet = false;
    }

    protected abstract void CheckWaitCondition();
}

public class WaitForTimeStep : WaitForStep
{
    public float waitTime = 1f;

    private float elapsedTime = 0;

    public override void ResetStep()
    {
        base.ResetStep();

        elapsedTime = 0;
    }

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetObject = null)
    {
        elapsedTime = 0;

        return base.ExecuteStep(attacker, target, originalPosition, callback);
    }

    protected override void CheckWaitCondition()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= waitTime)
        {
            waitConditionMet = true;
        }
    }
}

public class WaitForDistanceStep : WaitForStep
{
    public float distance = 1f;
    public bool useThingTop = false;

    private GameThing attackerThing = null, targetThing = null;

    private float currentDistance = 0f;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetObject = null)
    {
        currentDistance = 0f;

        attackerThing = attacker;
        targetThing = targetObject;

        return base.ExecuteStep(attacker, target, originalPosition, callback);
    }

    protected override void CheckWaitCondition()
    {
        if (attackerThing != null && targetThing != null)
        {
            currentDistance = Vector2.Distance(attackerThing.transform.position, (useThingTop) ? targetThing.thingTop.position : targetThing.transform.position);

            if (currentDistance >= distance)
            {
                waitConditionMet = true;
            }
        }
    }
}

public class WaitForProximityStep : WaitForStep
{
    public float distance = 1f;
    public bool useThingTop = false;

    private GameThing attackerThing = null, targetedThing = null;

    private float currentDistance = 0f;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        currentDistance = 0f;

        attackerThing = attacker;
        targetedThing = targetThing;

        return base.ExecuteStep(attacker, target, originalPosition, callback);
    }

    protected override void CheckWaitCondition()
    {
        if (attackerThing != null && targetedThing != null)
        {
            currentDistance = Vector2.Distance(attackerThing.transform.position, (useThingTop) ? targetedThing.thingTop.position : targetedThing.transform.position);

            if (currentDistance <= distance)
            {
                waitConditionMet = true;
            }
        }
    }
}
