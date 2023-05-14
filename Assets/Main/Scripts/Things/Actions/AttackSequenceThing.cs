using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSequenceThing : GameThing
{
    public override string thingType => "AttackSequence";

    [Min(0f)] public float range = 1f;
    public bool canUseEmptyTarget = false;
    public AttackStep[] attackSteps;
    public UnityEvent<bool> AttackSequenceFinished;


    private int currentStepIndex = 0;

    public void StartAttack(GameThing attacker, GameThing target)
    {
        ResetAttackSequence();
        StartCoroutine(AttackCoroutine(attacker, target, target.transform.position));
    }

    public void StartAttack(GameThing attacker, Vector2 targetPosition)
    {
        ResetAttackSequence();
        if (GameManager.instance)
            StartCoroutine(AttackCoroutine(attacker, null, targetPosition));
    }

    private void ResetAttackSequence()
    {
        // Stop the ongoing attack sequence if any
        StopAllCoroutines();
        // Reset the current step index
        currentStepIndex = 0;
    }

    private IEnumerator AttackCoroutine(GameThing attacker, GameThing target, Vector3 targetPosition)
    {
        bool allStepsSuccessful = true;

        currentStepIndex = 0;

        foreach (AttackStep step in attackSteps)
        {
            if (step == null)
                continue;

            step.ResetStep();

            StepResult result = StepResult.Failure;
            yield return StartCoroutine(step.ExecuteStep(attacker, target, stepResult => result = stepResult));

            if (result == StepResult.Failure)
            {
                allStepsSuccessful = false;
                break;
            }

            currentStepIndex++;
            // Debug.Log("Step " + currentStepIndex + " finished with result: " + result);
        }

        AttackSequenceFinished?.Invoke(allStepsSuccessful);
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
