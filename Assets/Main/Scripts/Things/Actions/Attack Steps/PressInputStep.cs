using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressInputStep : AttackStep
{
    [SerializeField] private InputActionReference inputActionReference;
    public Vector2Int requiredDirection;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
        bool inputSuccess = false;

        inputActionReference.action.Enable();

        while (!inputSuccess)
        {
            if (inputActionReference.action.GetType().Equals(InputActionType.Value) && inputActionReference.action.GetType().Equals(typeof(Vector2)))
            {
                Vector2 currentDirection = inputActionReference.action.ReadValue<Vector2>();
                Vector2Int currentDirectionInt = new Vector2Int(Mathf.RoundToInt(currentDirection.x), Mathf.RoundToInt(currentDirection.y));

                if (currentDirectionInt == requiredDirection)
                {
                    inputSuccess = true;
                }
            }
            else if (inputActionReference.action.GetType().Equals(InputActionType.Button))
            {
                if (inputActionReference.action.ReadValue<float>() > 0)
                {
                    inputSuccess = true;
                }
            }

            yield return null;
        }

        inputActionReference.action.Disable();

        callback?.Invoke(StepResult.Success);
    }
}
