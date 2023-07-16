using UnityEngine;

public class CharacterSpawner : GameThing
{
    public override string thingType => "CharacterSpawner";

    public string characterInfo;
    public bool spawnAtStart = true;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        GameManager.instance?.level.characterSpawners.Add(this);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        GameManager.instance?.level.characterSpawners.Remove(this);
    }

    public void SpawnCharacter()
    {
        if (string.IsNullOrEmpty(characterInfo))
            return;
        

        if (Instantiate(GameManager.instance?.characterPrefab, transform.position, Quaternion.identity).TryGetComponent(out CharacterThing character))
        {
            character.characterInfo = CharacterThing.CharacterInfo.FromString(characterInfo);
        }
    }
}