using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Color List")]
public class ColorList : ScriptableObject
{
    public List<Color> colors;

    // This is a helper method to assemble a list of colors from a string, taking the form of individual hex values,
    // separated by spaces. This is useful for quickly creating a list of colors in the inspector.
    [Sirenix.OdinInspector.Button]
    private void GetColorsFromString(string hexValues)
    {
        UnityEditor.Undo.RecordObject(this, "Get Colors From String");

        // Split the string into an array of individual hex values
        string[] hexArray = hexValues.Split(' ');

        // Clear the existing list of colors
        colors.Clear();

        // Iterate through the array of hex values and convert each one to a Color object
        for (int i = 0; i < hexArray.Length; i++)
        {
            Color color = HexToColor(hexArray[i]);
            colors.Add(color);
        }
    }

    // Helper function to convert a hex value to a Color object
    private Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}