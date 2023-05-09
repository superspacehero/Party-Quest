using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldInputStep : AttackStep
{
    [SerializeField] private InputActionReference inputActionReference;
    public Vector2Int requiredDirection;
    public float requiredHoldTime;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
        float elapsedTime = 0f;

        inputActionReference.action.Enable();

        while (elapsedTime < requiredHoldTime)
        {
            if (inputActionReference.action.GetType() == typeof(Vector2))
            {
                Vector2 currentDirection = inputActionReference.action.ReadValue<Vector2>();
                Vector2Int currentDirectionInt = new Vector2Int(Mathf.RoundToInt(currentDirection.x), Mathf.RoundToInt(currentDirection.y));

                if (currentDirectionInt == requiredDirection)
                {
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    elapsedTime = 0f;
                }
            }
            else if (inputActionReference.action.GetType() == typeof(float))
            {
                if (inputActionReference.action.ReadValue<float>() > 0)
                {
                    elapsedTime += Time.deltaTime;
                }
                else
                {
                    elapsedTime = 0f;
                }
            }

            yield return null;
        }

        inputActionReference.action.Disable();

        callback?.Invoke(StepResult.Success);
    }
}
