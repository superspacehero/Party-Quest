using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public enum GameMode
{
    Other,
    Play,
    Make
}

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

    #region Game variables

    public static GameMode gameMode = GameMode.Other;

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

    public GameObject characterPrefab;

    [SerializeField] private List<StartingActionThing> playerStartingActions = new List<StartingActionThing>();

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

    public float changeCharacterDelay = 1f;

    #endregion

    #region Things

    public DicePool dicePool;

    [SerializeField] private General.ObjectPool<UnityEngine.VFX.VisualEffect> damagePool, footstepPool;
    public void DamageEffect(int damageAmount, Vector3 position)
    {
        UnityEngine.VFX.VisualEffect damageEffect = damagePool.GetObjectFromPool(position);
        damageEffect.SetInt("Damage", damageAmount);
        damageEffect.Play();
    }

    public void FootstepEffect(Vector3 position)
    {
        UnityEngine.VFX.VisualEffect footstepEffect = footstepPool.GetObjectFromPool(position);
        footstepEffect.SetInt("Smoke Count", 1);
        footstepEffect.Play();
    }

    public void LandingEffect(Vector3 position)
    {
        UnityEngine.VFX.VisualEffect landingEffect = footstepPool.GetObjectFromPool(position);
        landingEffect.ResetOverride("Smoke Count");
        landingEffect.Play();
    }

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

    public static CharacterThing GetCharacterAtPosition(Vector3 position)
    {
        foreach (CharacterThing character in characters)
        {
            if (character.transform.position == position)
                return character;
        }

        return null;
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

    public static void SetAllPlayersCanControl(bool canControl)
    {
        foreach (ThingInput player in inputs)
            player.canControl = canControl;
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
        currentCharacter.input.AttachThing(currentCharacter);

        foreach (ThingInput player in GameManager.inputs)
        {
            player.canControl = currentCharacter.input == player;
        }

        if (instance.playerStartingActions.Count > 0)
        {
            foreach (ThingInput player in inputs)
            {
                if (player.GetAttachedThing() is CharacterThing)
                {
                    CharacterThing character = player.GetAttachedThing() as CharacterThing;

                    if (character.overrideStartingAction != null)
                    {
                        // Remove the override starting action
                        Destroy(character.overrideStartingAction);
                        character.overrideStartingAction = null;
                    }

                    // Add the override starting action
                    if (instance.playerStartingActions.Count > GameManager.inputs.IndexOf(player))
                        character.overrideStartingAction = character.gameObject.AddComponent(instance.playerStartingActions[GameManager.inputs.IndexOf(player)].GetType()) as StartingActionThing;
                    else
                        character.overrideStartingAction = character.gameObject.AddComponent(instance.playerStartingActions[instance.playerStartingActions.Count - 1].GetType()) as StartingActionThing;
                }
            }
        }

        currentCharacter.MyTurn();
    }

    #endregion

    #endregion

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {        
        // Spawn the players
        SpawnPlayers();
    }

    private PlayerInputManager playerInputManager
    {
        get
        {
            if (_playerInputManager == null)
                _playerInputManager = GetComponentInChildren<PlayerInputManager>();
            return _playerInputManager;
        }
    }
    private PlayerInputManager _playerInputManager;

    public void SpawnPlayer(PlayerAndCharacter player, PlayerSpawner playerSpawner = null)
    {
        if (playerInputManager == null)
            return;

        if (playerSpawner == null)
        {
            // Get all player spawners
            PlayerSpawner[] playerSpawners = FindObjectsOfType<PlayerSpawner>();
            if (playerSpawners.Length <= 0)
                return;

            // Choose a random player spawner
            playerSpawner = playerSpawners[Random.Range(0, playerSpawners.Length)];
        }

        if (playerInputManager.JoinPlayer(-1, -1, null, player.player).TryGetComponent(out ThingInput input))
        {
            // Create the character
            if (Instantiate(characterPrefab).TryGetComponent(out CharacterThing character))
            {
                // Move the character

                // First, get the character's rigidbody constraints
                RigidbodyConstraints constraints = character.movementController.rb.constraints;
                // Then, freeze the rotation
                character.movementController.rb.constraints = RigidbodyConstraints.FreezeRotation;
                // Move the character
                General.DelayedFunctionFrames(character, () => character.transform.position = playerSpawner.transform.position, 1);
                // Reset the character's rigidbody constraints
                character.movementController.rb.constraints = constraints;

                character.characterInfo = player.character;
                character.team = player.team;

                // Add the character to the player's input
                input.AttachThing(character);
                character.input = input;
            }
        }
    }

    [SerializeField] private GameObject cpuPlayerPrefab;

    public void SpawnCPUPlayer()
    {
        
    }

    public void SpawnPlayers()
    {
        PlayerSpawner[] playerSpawners = FindObjectsOfType<PlayerSpawner>();
        List<PlayerSpawner> playerSpawnersList = new List<PlayerSpawner>(playerSpawners);

        if (playerSpawners.Length <= 0)
            return;

        // Create the players
        foreach (PlayerAndCharacter player in players)
        {
            // Get a random player spawner
            if (playerSpawnersList.Count <= 0)
                playerSpawnersList = new List<PlayerSpawner>(playerSpawners);
            PlayerSpawner playerSpawner = playerSpawnersList[Random.Range(0, playerSpawnersList.Count)];

            // Spawn the player
            SpawnPlayer(player, playerSpawner);

            // Remove the player spawner from the list
            playerSpawnersList.Remove(playerSpawner);
        }
    }
}
