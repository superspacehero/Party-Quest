using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using I2.Loc;

public class TextList : MonoBehaviour
{
    [System.Serializable]
    public class TextListList
    {      
        [System.Serializable]
        public class TextListEntry
        {
            public bool isStruckOut;

            public string text;
            public string GetText()
            {
                return (isStruckOut ? "<s>" : "") + text + (isStruckOut ? "</s>" : "");
            }
        }

        public LocalizedString title;
        public List<TextListEntry> entries = new List<TextListEntry>();

        public string GetText()
        {
            string text = "";

            text += title.ToString() + "\n";
            foreach (TextListEntry entry in entries)
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

    public List<TextListList> strings;
    private string compiledString;

    [Button]
    private void CompileString()
    {
        compiledString = "";
        foreach (TextListList entry in strings)
            compiledString += entry.GetText() + "\n";

        text.text = compiledString;
    }
}
