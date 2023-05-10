using System.Collections;
using UnityEngine;

public abstract class WaitForStep : AttackStep
{
    protected bool waitConditionMet = false;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
        while (!waitConditionMet)
        {
            yield return null;
        }

        Debug.Log("WaitForStep wait condition met.");

        callback?.Invoke(StepResult.Success);
    }

    protected abstract void CheckWaitCondition();
}

public class WaitForTimeStep : WaitForStep
{
    public float waitTime = 1f;

    private float elapsedTime = 0;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
        elapsedTime = 0;

        return base.ExecuteStep(attacker, target, callback);
    }

    private void Update()
    {
        CheckWaitCondition();
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

public class WaitForCollisionStep : WaitForStep
{
    public LayerMask collisionLayers;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            CheckWaitCondition();
        }
    }

    protected override void CheckWaitCondition()
    {
        waitConditionMet = true;
    }
}
