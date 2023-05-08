using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldInputStep : AttackStep
{
    [SerializeField] private InputActionReference holdInputAction;
    public float requiredHoldTime;

    public override IEnumerator ExecuteStep(CharacterThing attacker, CharacterThing target, System.Action<StepResult> callback)
    {
        float elapsedTime = 0f;

        holdInputAction.action.Enable();

        while (elapsedTime < requiredHoldTime)
        {
            if (holdInputAction.action.ReadValue<float>() > 0)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                elapsedTime = 0f;
            }

            yield return null;
        }

        holdInputAction.action.Disable();

        callback?.Invoke(StepResult.Success);
    }
}
