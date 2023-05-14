using System.Collections;
using UnityEngine;

public abstract class WaitForStep : AttackStep
{
    protected bool waitConditionMet = false;

    public override IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback)
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

    public override IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback)
    {
        elapsedTime = 0;

        return base.ExecuteStep(attacker, target, callback);
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

    public override IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback)
    {
        currentDistance = 0f;

        attackerThing = attacker;
        targetThing = target;

        return base.ExecuteStep(attacker, target, callback);
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

    private GameThing attackerThing = null, targetThing = null;

    private float currentDistance = 0f;

    public override IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback)
    {
        currentDistance = 0f;

        attackerThing = attacker;
        targetThing = target;

        return base.ExecuteStep(attacker, target, callback);
    }

    protected override void CheckWaitCondition()
    {
        if (attackerThing != null && targetThing != null)
        {
            currentDistance = Vector2.Distance(attackerThing.transform.position, (useThingTop) ? targetThing.thingTop.position : targetThing.transform.position);

            if (currentDistance <= distance)
            {
                waitConditionMet = true;
            }
        }
    }
}
