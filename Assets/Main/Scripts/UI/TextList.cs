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

        public string titleSingular, titlePlural;
        public List<TextEntry> entries = new List<TextEntry>();

        public string GetText()
        {
            string text = "";

            text += (entries.Count == 1 ? titleSingular : titlePlural).ToString() + "\n";
            foreach (TextEntry entry in entries)
                text += entry.GetText() + "\n";

            return text;
        }

        public TextSection(string titleSingular, string titlePlural)
        {
            this.titleSingular = titleSingular;
            this.titlePlural = titlePlural;
        }
    }
    public List<TextSection> sections;
    public TextSection AddNewSection(string titleSingular, string titlePlural)
    {
        TextSection section = new TextSection(titleSingular, titlePlural);

        sections.Add(section);

        return section;
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

    private string compiledString;

    public void CompileString()
    {
        compiledString = "";
        foreach (TextSection entry in sections)
        {
            compiledString += entry.GetText();

            if (sections.IndexOf(entry) < sections.Count - 1)
                compiledString += "\n";
        }

        text.text = compiledString;
    }

    public void Clear()
    {
        sections.Clear();
    }
}
