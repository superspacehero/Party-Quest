using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectMenu : MonoBehaviour
{
    public static CharacterSelectMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CharacterSelectMenu>();
            }
            return _instance;
        }
    }
    private static CharacterSelectMenu _instance;

    public Transform characterSelectParent
    {
        get
        {
            if (_characterSelectParent == null)
            {
                _characterSelectParent = transform;
            }
            return _characterSelectParent;
        }
    }
    [SerializeField] private Transform _characterSelectParent;
}
