using UnityEngine;

public class CharacterSpawner : GameThing
{
    public CharacterThing.CharacterInfo characterInfo;
    public int team;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (GameManager.instance && GameManager.instance.characterPrefab)
        {
            if (Instantiate(GameManager.instance.characterPrefab).TryGetComponent(out CharacterThing character))
            {
                character.characterInfo = characterInfo;
                character.team = team;

                // Add the character to the player's input
                AttachThing(character);
                character.OccupyCurrentNode();
            }
        }
    }
}