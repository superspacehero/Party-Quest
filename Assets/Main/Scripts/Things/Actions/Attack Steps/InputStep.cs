using System.Collections;
using UnityEngine;

public enum RequiredInput
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Primary,
    Secondary
}

public class InputStep : AttackStep
{
    public RequiredInput requiredInput;
    public bool wrongInputFailsStep = true;
    [Min(0)] public float minHoldTime = 0;
    [Min(0)] public float maxHoldTime = Mathf.Infinity;
    [Min(0)] public float maxWaitTime = 0.1f;
    public bool pressInput = true;   // If true, the input is successful when the button is pressed
    public bool holdInput = false;   // If true, the input is successful when the button is held down

    private bool? inputSuccess = null;
    private float elapsedTime = 0;

    public override IEnumerator ExecuteStep(GameThing attacker, Vector3 target, System.Action<StepResult> callback, GameThing targetObject = null)
    {
        float waitTime = 0;

        while (inputSuccess == null)
        {
            if (inputSuccess.HasValue && inputSuccess.Value)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= maxHoldTime)
                {
                    inputSuccess = false;
                }
            }
            else
            {
                waitTime += Time.deltaTime;
                if (waitTime >= maxWaitTime)
                {
                    inputSuccess = false;
                }
            }

            yield return null;
        }

        callback?.Invoke(inputSuccess.Value ? StepResult.Success : StepResult.Failure);
    }

    public override void ResetStep()
    {
        inputSuccess = null;
        elapsedTime = 0;
    }

    public override void Move(Vector2 direction)
    {
        Vector2Int currentDirectionInt = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

        if (requiredInput == RequiredInput.MoveUp && currentDirectionInt == Vector2Int.up ||
            requiredInput == RequiredInput.MoveDown && currentDirectionInt == Vector2Int.down ||
            requiredInput == RequiredInput.MoveLeft && currentDirectionInt == Vector2Int.left ||
            requiredInput == RequiredInput.MoveRight && currentDirectionInt == Vector2Int.right)
        {
            if (pressInput)
                inputSuccess = true;

            if (holdInput)
                elapsedTime += Time.deltaTime;
            if (elapsedTime >= minHoldTime && elapsedTime <= maxHoldTime)
                inputSuccess = true;
        }
        else if (wrongInputFailsStep && direction != Vector2.zero)
        {
            inputSuccess = false;
        }
    }

    public override void PrimaryAction(bool pressed)
    {
        if (requiredInput == RequiredInput.Primary && pressed)
        {
            if (pressInput)
                inputSuccess = true;

            if (holdInput)
                elapsedTime += Time.deltaTime;
            if (elapsedTime >= minHoldTime && elapsedTime <= maxHoldTime)
                inputSuccess = true;
        }
        else if (wrongInputFailsStep && pressed)
        {
            inputSuccess = false;
        }
    }

    public override void SecondaryAction(bool pressed)
    {
        if (requiredInput == RequiredInput.Secondary && pressed)
        {
            if (pressInput)
                inputSuccess = true;

            if (holdInput)
                elapsedTime += Time.deltaTime;
            if (elapsedTime >= minHoldTime && elapsedTime <= maxHoldTime)
                inputSuccess = true;
        }
        else if (wrongInputFailsStep && pressed)
        {
            inputSuccess = false;
        }
    }
}
