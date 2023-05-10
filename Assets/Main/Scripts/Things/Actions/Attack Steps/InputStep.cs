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

    private bool? inputSuccess = null;
    private float elapsedTime = 0;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
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

            yield return null;
        }

        callback?.Invoke(inputSuccess.Value ? StepResult.Success : StepResult.Failure);
    }

    public override void Move(Vector2 direction)
    {
        Vector2Int currentDirectionInt = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

        if (requiredInput == RequiredInput.MoveUp && currentDirectionInt == Vector2Int.up ||
            requiredInput == RequiredInput.MoveDown && currentDirectionInt == Vector2Int.down ||
            requiredInput == RequiredInput.MoveLeft && currentDirectionInt == Vector2Int.left ||
            requiredInput == RequiredInput.MoveRight && currentDirectionInt == Vector2Int.right)
        {
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
            inputSuccess = true;
        }
        else if (wrongInputFailsStep && pressed)
        {
            inputSuccess = false;
        }
    }
}
