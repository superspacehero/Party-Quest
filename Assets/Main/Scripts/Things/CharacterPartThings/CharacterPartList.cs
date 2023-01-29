using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPartList", menuName = "Character Part List")]
public class CharacterPartList : ScriptableObject
{
    public List<GameObject> characterParts = new List<GameObject>();
}
