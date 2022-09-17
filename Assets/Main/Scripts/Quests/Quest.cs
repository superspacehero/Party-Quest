using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using I2.Loc;
using Sirenix.OdinInspector;

[System.Serializable]
public class Quest
{
    
    [System.Serializable]
    public struct Info
    {
        public string name;
        public Sprite icon;
        public string description;
    }

    [System.Serializable]
    public struct Stat
    {
        public int currency;
        public List<Thing> things;
        public int exp;

        public Stat(int currency, List<Thing> things, int exp)
        {
            this.currency = currency;
            this.things = things;
            this.exp = exp;
        }
    }

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
    [System.Serializable] public class QuestCompletedEvent : UnityEvent<Quest> { }

    [Header("Quest Info")] public Info info;
    [Header("Reward")] public Stat reward = new Stat(100, new List<Thing>(), 100);

    public List<QuestGoal> mainGoals = new List<QuestGoal>(), optionalGoals = new List<QuestGoal>();

    public bool completed { get; private set; }
    public QuestCompletedEvent questCompleted = new QuestCompletedEvent();

    public void Initialize()
    {
        completed = false;
        questCompleted = new QuestCompletedEvent();

        foreach (QuestGoal goal in mainGoals)
        {
            goal.Initialize();
            goal.goalCompleted.AddListener(call:delegate { CheckGoals(); });
        }

        foreach (QuestGoal goal in optionalGoals)
            goal.Initialize();
    }

    private void CheckGoals()
    {
        completed = mainGoals.FindAll(goal => goal.completed).Count == mainGoals.Count;

        if (completed)
        {
            questCompleted.Invoke(this);
            questCompleted.RemoveAllListeners();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor
{
    SerializedProperty _questInfoProperty, _questRewardProperty;
    List<string> _questGoalTypes = new List<string>();
    SerializedProperty _mainGoalsProperty, _optionalGoalsProperty;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        _questInfoProperty = serializedObject.FindProperty(nameof(Quest.info));
        _questRewardProperty = serializedObject.FindProperty(nameof(Quest.reward));

        _mainGoalsProperty = serializedObject.FindProperty(nameof(Quest.mainGoals));
        _optionalGoalsProperty = serializedObject.FindProperty(nameof(Quest.optionalGoals));

        var lookup = typeof(Quest.QuestGoal);
        _questGoalTypes = new List<string>(System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => lookup.IsAssignableFrom(p) && !p.IsAbstract)
            .Select(t => t.Name));
    }

    public override void OnInspectorGUI()
    {
        var child = _questInfoProperty.Copy();
        var depth = child.depth;
        child.NextVisible(true);

        EditorGUILayout.LabelField("Quest Info", EditorStyles.boldLabel);
        while (child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }

        child = _questRewardProperty.Copy();
        depth = child.depth;
        child.NextVisible(true);

        EditorGUILayout.LabelField("Quest Reward", EditorStyles.boldLabel);
        while (child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }
    }
}
#endif