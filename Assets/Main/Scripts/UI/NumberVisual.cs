using UnityEngine;

public class NumberVisual : MonoBehaviour
{
    public int number
    {
        get => _number;
        set
        {
            _number = value;
            text.text = value.ToString();
        }
    }
    private int _number;

    private TMPro.TextMeshProUGUI text
    {
        get
        {
            if (_text == null)
                _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();

            return _text;
        }
    }
    private TMPro.TextMeshProUGUI _text;

    public FollowWorldPosition followWorldPosition
    {
        get
        {
            if (_followWorldPosition == null)
                TryGetComponent(out _followWorldPosition);

            return _followWorldPosition;
        }
    }
    private FollowWorldPosition _followWorldPosition;
}
