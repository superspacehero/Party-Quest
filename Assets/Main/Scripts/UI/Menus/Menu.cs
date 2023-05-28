using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour
{
    #region Static Variables

    public static Menu currentMenuOption
    {
        get { return _currentMenuOption; }
        set
        {
            if (value == _currentMenuOption)
                return;

            _currentMenuOption = value;

            DeselectMenuOption(exceptingMenuOption: value);
        }
    }
    private static Menu _currentMenuOption;

    private CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                TryGetComponent(out _canvasGroup);

            return _canvasGroup;
        }
    }
    private CanvasGroup _canvasGroup;

    public static List<Menu> menuOptions = new List<Menu>();

    private static void FindMenuOptions()
    {
        foreach (Menu menuOption in FindObjectsOfType<Menu>())
            menuOptions.Add(menuOption);
    }

    private static void DeselectMenuOption(Menu exceptingMenuOption)
    {
        if (exceptingMenuOption == null)
            return;

        foreach (Menu option in menuOptions)
            if (option != exceptingMenuOption && option.menuGroup == exceptingMenuOption.menuGroup)
                option.Deselect();
    }

    #endregion

    #region Variables

    protected GameObject objectToSelect
    {
        get
        {
            if (_objectToSelect == null)
                _objectToSelect = gameObject;

            return _objectToSelect;
        }

        set
        {
            _objectToSelect = value;
            if (enabled && _objectToSelect != null && _objectToSelect.TryGetComponent(out Selectable _selectable))
                _selectable.Select();
        }
    }
    [SerializeField, FoldoutGroup("Variables")]
    protected GameObject _objectToSelect;

    [FoldoutGroup("Variables"), Min(0)]
    public int menuGroup;

    private bool selected;

    #endregion

    #region Other Menus

    [FoldoutGroup("Other Menus")]
    public Menu previousOption, nextOption;

#if UNITY_EDITOR
    [FoldoutGroup("Other Menus"), Button, ContextMenu("Set Connected Objects")]
    private void SetConnectedMenuObjects()
    {
        UnityEditor.EditorUtility.SetDirty(this);

        if (previousOption != null)
        {
            previousOption.nextOption = this;
            UnityEditor.EditorUtility.SetDirty(previousOption);
        }

        if (nextOption != null)
        {
            nextOption.previousOption = this;
            UnityEditor.EditorUtility.SetDirty(nextOption);
        }

        UnityEditor.Undo.RecordObject(this, "Set Connected Menu Objects");
    }
#endif

    [HorizontalGroup("Other Menus/Menus"), Button, ContextMenu("Previous Menu")]
    public virtual void PreviousMenu()
    {
        if (previousOption != null)
            previousOption.Select();
        else
            Debug.LogWarning("No previous menu found.");
    }

    [HorizontalGroup("Other Menus/Menus"), Button, ContextMenu("Next Menu")]
    public virtual void NextMenu()
    {
        if (nextOption != null)
            nextOption.Select();
        else
            Debug.LogWarning("No next menu found.");
    }

    #endregion

    [FoldoutGroup("Select Events")]
    public UnityEvent onSelect, onDeselect;

    protected List<Selectable> selectables
    {
        get
        {
            FindSelectables();

            return _selectables;
        }

        set { _selectables = value; }
    }
    protected List<Selectable> _selectables = new List<Selectable>();

    private void FindSelectables(bool clearList = false)
    {
        List<Selectable> selectableList = new List<Selectable>();

        for (int i = 0; i < _selectables.Count; i++)
        {
            if (_selectables[i] != null)
                selectableList.Add(_selectables[i]);
        }

        foreach (Selectable selectable in GetComponentsInChildren<Selectable>())
            if (!selectableList.Contains(selectable))
                selectableList.Add(selectable);

        selectables = selectableList;
    }

    [FoldoutGroup("Selectables"), Button, ContextMenu("Find Selectables (Keep Existing Ones)"), Title("Find Selectables", null, TitleAlignments.Centered)]
    private void KeepSelectables()
    {
        FindSelectables(clearList: false);
    }

    [FoldoutGroup("Selectables"), Button, ContextMenu("Find Selectables (Clear Existing Ones)")]
    private void ClearSelectables()
    {
        FindSelectables(clearList: true);
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    public virtual void OnEnable()
    {
        // If we're in the editor, we don't need to do anything

        if (Application.isPlaying)
        {
            if (!menuOptions.Contains(this))
                menuOptions.Add(this);

            StartCoroutine(SelectSelf());
        }
    }

    protected EventTrigger eventTrigger
    {
        get
        {
            if (_eventTrigger == null)
                TryGetComponent(out _eventTrigger);

            return _eventTrigger;
        }
    }
    protected EventTrigger _eventTrigger;
    private bool hasEventTrigger
    {
        get
        {
            if (eventTrigger == null)
                return false;

            return true;
        }
    }
    [SerializeField, ShowIf("hasEventTrigger"), FoldoutGroup("Event Trigger")]
    protected bool previousMenuOnCancel = true;

#if UNITY_EDITOR
    [Button, ContextMenu("Add Event Trigger"), HideIf("hasEventTrigger"), FoldoutGroup("Event Trigger")]
    private void AddEventTrigger()
    {
        if (TryGetComponent(out EventTrigger eventTrigger))
            return;

        gameObject.AddComponent<EventTrigger>();
    }
#endif

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public void Start()
    {
        // If we have an event trigger and we want to go to the previous menu on cancel, we add the event trigger
        if (eventTrigger && previousMenuOnCancel)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Cancel;
            entry.callback.AddListener((data) => { PreviousMenu(); });

            eventTrigger.triggers.Add(entry);
        }
    }

    IEnumerator SelectSelf()
    {
        yield return null;
        Select();
    }
    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    public virtual void OnDisable()
    {
        if (Application.isPlaying)
            Deselect();
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (Application.isPlaying && menuOptions.Contains(this))
            menuOptions.Remove(this);
    }

    [Button, ContextMenu("Select")]
    public virtual void Select()
    {
        if (selected)
            return;

        if (!gameObject.activeInHierarchy || !enabled)
        {
            gameObject.SetActive(true);
            enabled = true;

            Debug.Log("Enabling " + gameObject.name + " for selection");

            return;
        }

        if (canvasGroup)
            canvasGroup.interactable = true;

        if (objectToSelect != null && objectToSelect.TryGetComponent(out Selectable selectable))
            selectable.Select();

        onSelect.Invoke();

        selected = true;
        currentMenuOption = this;
    }

    public void Deselect()
    {
        if (!selected)
            return;

        onDeselect.Invoke();

        if (canvasGroup)
            canvasGroup.interactable = false;

        selected = false;
    }
}
