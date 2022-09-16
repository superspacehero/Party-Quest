using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using I2.Loc;

public class TextList : MonoBehaviour
{
    
    [System.Serializable]
    public class TextSection
    {      
        [System.Serializable]
        public class TextEntry
        {
            public string text;
            [SerializeField] private string textToPrepend = "- ";
            public bool isStruckOut;

            public string GetText()
            {
                return (isStruckOut ? "<s>" : "") + textToPrepend + text + (isStruckOut ? "</s>" : "");
            }
        }

        public LocalizedString title;
        public List<TextEntry> entries = new List<TextEntry>();

        public string GetText()
        {
            string text = "";

            text += title.ToString() + "\n";
            foreach (TextEntry entry in entries)
                text += entry.GetText() + "\n";

            return text;
        }
    }

    private TextMeshProUGUI text
    {
        get
        {
            if (_text == null);
                TryGetComponent(out _text);

            return _text;
        }
    }
    private TextMeshProUGUI _text;

    public List<TextSection> strings;
    private string compiledString;

    [Button]
    private void CompileString()
    {
        compiledString = "";
        foreach (TextSection entry in strings)
        {
            compiledString += entry.GetText();

            if (strings.IndexOf(entry) < strings.Count - 1)
                compiledString += "\n";
        }

        text.text = compiledString;
    }
}
