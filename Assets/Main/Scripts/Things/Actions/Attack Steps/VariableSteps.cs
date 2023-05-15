using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SetVariableStep : AttackStep
{
    public GameThing.GameThingVariables variables;

    public bool useTarget = false;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        if (targetThing != null && useTarget)
            targetThing.variables += variables;
        else
            attacker.variables += variables;

        callback?.Invoke(StepResult.Success);

        yield return null;
    }

    public override void ResetStep()
    {
    }
}

public class VariableBranchStep : AttackStep
{
    public GameThing.GameThingVariables.Variable variable;

    public bool useTarget = false;

    public enum BranchType
    {
        GreaterThan,
        LessThan,
        EqualTo
    }

    public BranchType branchType;

    public AttackStep[] trueSteps, falseSteps;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, Action<StepResult> callback, GameThing targetThing = null)
    {
        int var = (targetThing != null && useTarget) ? targetThing.variables.GetVariable(variable.name) : attacker.variables.GetVariable(variable.name);
        
        if (branchType == BranchType.GreaterThan)
        {
            if (var > variable.value)
            {
                foreach (AttackStep step in trueSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
            else
            {
                foreach (AttackStep step in falseSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
        }
        else if (branchType == BranchType.LessThan)
        {
            if (var < variable.value)
            {
                foreach (AttackStep step in trueSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
            else
            {
                foreach (AttackStep step in falseSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
        }
        else if (branchType == BranchType.EqualTo)
        {
            if (var == variable.value)
            {
                foreach (AttackStep step in trueSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
            else
            {
                foreach (AttackStep step in falseSteps)
                {
                    yield return step.ExecuteStep(attacker, target, callback, targetThing);
                }
            }
        }

        callback?.Invoke(StepResult.Success);
    }

    public override void ResetStep()
    {
    }
}

public class DamageStep : AttackStep
{
    public bool overrideDamage = false;
    [ShowIf(nameof(overrideDamage))] public int damage = 0;

    public bool useTarget = false;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, System.Action<StepResult> callback, GameThing targetThing = null)
    {
        GameThing targetedThing = (target != null && useTarget) ? targetThing : attacker;

        targetedThing.variables.SetVariable("health", (overrideDamage) ? targetThing.variables.GetVariable("health") - damage : targetThing.variables.GetVariable("health") - Mathf.Max(0, attacker.variables.GetVariable("attack") - targetThing.variables.GetVariable("defense")));

        callback?.Invoke(StepResult.Success);

        yield return null;
    }

    public override void ResetStep()
    {
    }
}