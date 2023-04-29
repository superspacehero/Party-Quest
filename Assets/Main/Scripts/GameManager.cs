using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();

            return _instance;
        }
    }
    private static GameManager _instance;

    #endregion

    #region Level

    public static string levelString;
    public Level level = new Level();

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Level.Room room in level.rooms)
        {
            Gizmos.DrawWireCube((Vector3)(room.min + room.max) / 2, (Vector3)(room.max - room.min));
        }
    }

    #endregion

    #region Players

    [SerializeField] private GameObject characterPrefab;

    public struct PlayerAndCharacter
    {
        // The device that the player is using
        public InputDevice player;

        // The character that the player is controlling
        public CharacterThing.CharacterInfo character;

        // The team that the player is on
        public int team;

        public PlayerAndCharacter(PlayerInput playerInput, CharacterThing.CharacterInfo character, int team = 0)
        {
            player = playerInput.devices[0];
            this.character = character;
            this.team = team;
        }
    }
    public static List<PlayerAndCharacter> players = new List<PlayerAndCharacter>();

    public static List<ThingInput> inputs = new List<ThingInput>();
    public static void AddPlayer(ThingInput player)
    {
        inputs.Add(player);
    }

    public static void RemovePlayer(ThingInput player)
    {
        inputs.Remove(player);
    }

    public static void SetAllPlayersCanControl(bool canControl)
    {
        foreach (ThingInput player in inputs)
            player.canControl = canControl;
    }

    public float changeCharacterDelay = 1f;

    #endregion

    #region Things

    public static List<GameThing> things = new List<GameThing>();

    #region Characters

    [SerializeField] private ThingDisplay nextCharacterUI;
    public static List<CharacterThing> characters = new List<CharacterThing>();
    public static int currentCharacterIndex = 0;

    public static CharacterThing currentCharacter
    {
        get
        {
            if (characters.Count <= 0)
                return null;

            return charactersInCurrentTeam[currentCharacterIndex];
        }
    }

    public static void AddCharacter(CharacterThing character)
    {
        characters.Add(character);
    }

    public static void RemoveCharacter(CharacterThing character)
    {
        characters.Remove(character);
    }

    #region Teams

    public static List<int> teams = new List<int>();
    public static List<CharacterThing> charactersInCurrentTeam = new List<CharacterThing>();

    public static int currentTeamIndex
    {
        get => _currentTeam;
        set
        {
            if (value >= teams.Count)
                value = 0;

            _currentTeam = value;

            charactersInCurrentTeam.Clear();
            foreach (var character in characters)
            {
                if (character.team == _currentTeam)
                    charactersInCurrentTeam.Add(character);
            }

            currentCharacterIndex = 0;
        }
    }
    private static int _currentTeam;

    #endregion

    [Button("Next Character")]
    private void NextCharacterButton()
    {
        NextCharacter();
    }

    public static void NextCharacter()
    {
        SetAllPlayersCanControl(false);
        General.DelayedFunctionSeconds(instance, SetNextCharacter, delaySeconds: instance.changeCharacterDelay);
    }
    private static void SetNextCharacter()
    {
        if (characters.Count == 0)
            return;

        currentCharacterIndex++;
        if (currentCharacterIndex >= charactersInCurrentTeam.Count)
            currentTeamIndex++;

        GameplayCamera.SetCameraObject(currentCharacter);
        instance.nextCharacterUI.thing = currentCharacter;
        instance.nextCharacterUI.gameObject.SetActive(true);
    }

    public static void ControlNextCharacter()
    {
        currentCharacter.MyTurn();

        foreach (ThingInput player in inputs)
            player.canControl = player.GetAttachedThing() == currentCharacter;
    }

    #endregion

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        foreach (PlayerAndCharacter player in players)
        {
            // Create the player
            if (GetComponentInChildren<PlayerInputManager>().TryGetComponent(out PlayerInputManager playerInputManager))
            {
                if (playerInputManager.JoinPlayer(-1, -1, null, player.player).TryGetComponent(out ThingInput input))
                {
                    // Create the character
                    if (Instantiate(characterPrefab).TryGetComponent(out CharacterThing character))
                    {
                        character.characterInfo = player.character;
                        character.team = player.team;

                        // Add the character to the player's input
                        input.AttachThing(character);
                    }
                }
            }
        }
    }
}
