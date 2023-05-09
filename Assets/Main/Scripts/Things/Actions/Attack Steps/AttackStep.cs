using UnityEngine;
using System.Collections;

public enum StepResult
{
    Success,
    Failure
}

public abstract class AttackStep
{
    public abstract IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback);
}