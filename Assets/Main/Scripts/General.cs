using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class General
{
    public enum Priority
    {
        highPriority,
        mediumPriority,
        lowPriority,
        topPriority
    }

    #region Cached Animations

        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Fall = Animator.StringToHash("Fall");
        public static readonly int Land = Animator.StringToHash("Land");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Crouch = Animator.StringToHash("Crouch");

    #endregion

    #region Delayed Functions

        public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        public static void DelayedFunctionFrames(MonoBehaviour monoBehaviour, System.Action action, int delayFrames = 0)
        {
            monoBehaviour.StartCoroutine(DelayedFunctionCoroutine(action, delayFrames));
        }

        public static void DelayedFunctionSeconds(MonoBehaviour monoBehaviour, System.Action action, float delaySeconds = 0)
        {
            monoBehaviour.StartCoroutine(DelayedFunctionCoroutine(action, Mathf.RoundToInt(delaySeconds / Time.fixedDeltaTime)));
        }

        public static IEnumerator DelayedFunctionCoroutine(System.Action action, int framesToDelay = 0)
        {
            if (framesToDelay > 1)
            {
                framesDelayed = 0;
                while (framesDelayed < framesToDelay)
                {
                    framesDelayed++;
                    yield return waitForFixedUpdate;
                }
            }
            else
                yield return waitForFixedUpdate;

            action();
        }
        private static int framesDelayed;

    #endregion
}
