using UnityEngine;
using System.Collections;

public enum StepResult
{
    Success,
    Failure
}

public abstract class AttackStep
{
    public abstract IEnumerator ExecuteStep(GameThing attacker, GameThing target, System.Action<StepResult> callback);

    public abstract void InitializeStep();

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