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

            set { _objectToSelect = value; }
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
    public void Select()
    {
        if (selected)
            return;

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            Debug.Log("Enabling " + gameObject.name + " for selection");

            return;
        }

        if (canvasGroup)
            canvasGroup.interactable = true;
        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(objectToSelect);

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
