using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using I2.Loc;
using Sirenix.OdinInspector;

[System.Serializable]
public abstract class QuestGoal
{
    [SerializeField] protected LocalizedString description;
    public int currentAmount { get; protected set; }
    public int requiredAmount = 1;

    public bool completed { get; protected set; }
    public UnityEvent goalCompleted = new UnityEvent();

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual void Initialize()
    {
        completed = false;
    }

    protected void Evaluate()
    {
        if (currentAmount >= requiredAmount)
            Complete();
    }

    private void Complete()
    {
        completed = true;
        goalCompleted.Invoke();
        goalCompleted.RemoveAllListeners();
    }
}