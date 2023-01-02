using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    private Color _selectedColor;

    [SerializeField] private ColorList colorList;
    [SerializeField] private GameObject colorPrefab;
    [SerializeField] private Transform colorContainer;

    void Start()
    {
        // Iterate through the list of colors and instantiate a button for each one
        for (int i = 0; i < colorList.colors.Count; i++)
        {
            if (Instantiate(colorPrefab, colorContainer).TryGetComponent(out Button button))
            {
                button.targetGraphic.color = colorList.colors[i];
                button.onClick.AddListener(() => SetSelectedColor(colorList.colors[i]));
            }
        }
    }

    public void SetSelectedColor(Color color)
    {
        _selectedColor = color;
    }
}