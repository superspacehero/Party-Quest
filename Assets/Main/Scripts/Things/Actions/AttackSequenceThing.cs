using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequenceThing : GameThing
{
    public override string thingType => "AttackSequence";

    [Min(0f)] public float range = 1f;
    public List<AttackStep> attackSteps;

    public void StartAttack(CharacterThing attacker, CharacterThing target)
    {
        StartCoroutine(AttackCoroutine(attacker, target));
    }

    private IEnumerator AttackCoroutine(CharacterThing attacker, CharacterThing target)
    {
        bool allStepsSuccessful = true;

        foreach (AttackStep step in attackSteps)
        {
            StepResult result = StepResult.Failure;
            yield return StartCoroutine(step.ExecuteStep(attacker, target, stepResult => result = stepResult));

            if (result == StepResult.Failure)
            {
                allStepsSuccessful = false;
                break;
            }
        }

        if (allStepsSuccessful)
        {
            // Handle success, e.g., deal damage or apply effects.
        }
        else
        {
            // Handle failure, e.g., display a message or play a sound.
        }
    }

    public override void Move(Vector2 direction)
    {
        // Implement character movement during attack sequence if needed.
    }

    public override void PrimaryAction(bool pressed)
    {
        // Implement primary action during attack sequence if needed.
    }

    public override void SecondaryAction(bool pressed)
    {
        // Implement secondary action during attack sequence if needed.
    }
}
