using UnityEngine;
using System.Collections;

public enum AttackTarget
{
    Attacker,
    Target,
    TargetThing
}

public enum StepResult
{
    Success,
    Failure
}

public abstract class AttackStep
{
    public abstract IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetThing = null);

    public abstract void ResetStep();

    public virtual void Move(Vector2 direction)
    {
        // Implement character movement during attack step if needed.
    }

    public virtual void PrimaryAction(bool pressed)
    {
        // Implement primary action during attack step if needed.
    }

    public virtual void SecondaryAction(bool pressed)
    {
        // Implement secondary action during attack step if needed.
    }
}

public class FailStep : AttackStep
{
    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Vector3 originalPosition, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        callback?.Invoke(StepResult.Failure);
        yield break;
    }

    public override void ResetStep()
    {
    }
}