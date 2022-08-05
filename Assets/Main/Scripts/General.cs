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
}
