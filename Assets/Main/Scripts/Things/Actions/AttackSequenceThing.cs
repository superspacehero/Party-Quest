using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSequenceThing : GameThing
{
    public override string thingType => "AttackSequence";

    [Min(0f)] public float range = 1f;
    public AttackStep[] attackSteps;

    private int currentStepIndex = 0;

    public void StartAttack(CharacterThing attacker, CharacterThing target)
    {
        StartCoroutine(AttackCoroutine(attacker, target));
    }

    private IEnumerator AttackCoroutine(CharacterThing attacker, CharacterThing target)
    {
        bool allStepsSuccessful = true;

        currentStepIndex = 0;

        foreach (AttackStep step in attackSteps)
        {
            StepResult result = StepResult.Failure;
            yield return StartCoroutine(step.ExecuteStep(attacker, target, stepResult => result = stepResult));

            if (result == StepResult.Failure)
            {
                allStepsSuccessful = false;
                break;
            }

            currentStepIndex++;
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

    private bool currentStepExists => (attackSteps.Length > 0 && currentStepIndex < attackSteps.Length && attackSteps[currentStepIndex] != null);

    public override void Move(Vector2 direction)
    {
        // Implement character movement during attack sequence if needed.
        if (currentStepExists)
            attackSteps[currentStepIndex].Move(direction);
    }

    public override void PrimaryAction(bool pressed)
    {
        // Implement primary action during attack sequence if needed.
        if (currentStepExists)
            attackSteps[currentStepIndex].PrimaryAction(pressed);
    }

    public override void SecondaryAction(bool pressed)
    {
        // Implement secondary action during attack sequence if needed.
        if (currentStepExists)
            attackSteps[currentStepIndex].SecondaryAction(pressed);
    }
}
