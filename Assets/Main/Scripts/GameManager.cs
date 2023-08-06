using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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

    [NaughtyAttributes.Scene]
    public string levelScene, levelEditorScene;

    public static GameMode gameMode = GameMode.Other;

    public Menu touchControls;

    public static void EnableTouchControls()
    {
        if (instance == null)
            return;

        instance.touchControls.Select();
    }

    #endregion

    #region Level

    [SerializeField] private LevelUI levelIntroUI, levelInfoUI;
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

    [SerializeField] private List<ActionThing> playerStartingActions = new List<ActionThing>();

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
            // If the player input is null, then the player is a CPU
            player = (playerInput != null) ? playerInput.devices[0] : null;
            this.character = character;
            this.team = team;
        }
    }
    public static List<PlayerAndCharacter> players = new List<PlayerAndCharacter>();

    public List<ThingInput> inputs = new List<ThingInput>();
    public static void AddPlayer(ThingInput player)
    {
        instance?.inputs.Add(player);
    }

    public static void RemovePlayer(ThingInput player)
    {
        instance?.inputs.Remove(player);
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
    public int currentCharacterIndex = 0;

    public static CharacterThing currentCharacter
    {
        get
        {
            if (instance.level.characters.Count <= 0)
                return null;

            return instance?.charactersInCurrentTeam[instance.currentCharacterIndex];
        }
    }

    public static void AddCharacter(CharacterThing character)
    {
        instance?.level.characters.Add(character);
    }

    public static void RemoveCharacter(CharacterThing character)
    {
        instance?.level.characters.Remove(character);
    }

    public static CharacterThing GetCharacterAtNode(Pathfinding.GraphNode node)
    {
        foreach (CharacterThing character in instance.level.characters)
        {
            if (character.currentNode == node)
                return character;
        }

        return null;
    }

    #region Teams

    [SerializeField] private List<int> teams = new List<int>();
    public List<CharacterThing> charactersInCurrentTeam = new List<CharacterThing>();
    public int currentTeamIndex
    {
        get => instance._currentTeamIndex;
        set
        {
            if (value >= teams.Count)
                value = 0;

            instance._currentTeamIndex = value;

            charactersInCurrentTeam.Clear();
            foreach (var character in instance.level.characters)
            {
                if (!teams.Contains(character.characterTeam))
                {
                    teams.Add(character.characterTeam);
                }

                if (character.characterTeam == instance._currentTeamIndex)
                {
                    charactersInCurrentTeam.Add(character);
                }
            }

            instance.currentCharacterIndex = 0;
        }
    }
    private int _currentTeamIndex;

    #endregion

    [Button("Next Character")]
    private void NextCharacterButton()
    {
        NextCharacter();
    }

    public static void NextCharacter()
    {
        SetAllPlayersCanControl(false);
        General.DelayedFunctionSeconds(instance, () => SetNextCharacter(true), delaySeconds: instance.changeCharacterDelay);
    }

    public static void SetAllPlayersCanControl(bool canControl)
    {
        foreach (ThingInput player in instance.inputs)
            player.canControl = canControl;
    }

    private static void SetNextCharacter(bool showNextCharacterUI = true)
    {
        if (instance.level.characters.Count == 0)
            return;

        instance.currentCharacterIndex++;
        if (instance.currentCharacterIndex >= instance.charactersInCurrentTeam.Count)
            instance.currentTeamIndex++;

        GameplayCamera.SetCameraObject(currentCharacter);
        instance.nextCharacterUI.thing = currentCharacter;

        if (showNextCharacterUI)
            instance.nextCharacterUI.gameObject.SetActive(true);
    }

    public static void ControlNextCharacter()
    {
        if (instance.playerStartingActions.Count > 0)
        {
            foreach (ThingInput player in instance.inputs)
            {
                foreach (Inventory.ThingSlot thingSlot in player.inventory.thingSlots)
                {
                    if (thingSlot.thing != null && thingSlot.thing is CharacterThing)
                    {
                        CharacterThing character = thingSlot.thing as CharacterThing;

                        if (character.overrideStartingAction != null)
                        {
                            // Remove the override starting action
                            Destroy(character.overrideStartingAction);
                            character.overrideStartingAction = null;
                        }

                        // Add the override starting action
                        if (instance.playerStartingActions.Count > instance.inputs.IndexOf(player))
                            character.overrideStartingAction = character.gameObject.AddComponent(instance.playerStartingActions[instance.inputs.IndexOf(player)].GetType()) as ActionThing;
                        else
                            character.overrideStartingAction = character.gameObject.AddComponent(instance.playerStartingActions[instance.playerStartingActions.Count - 1].GetType()) as ActionThing;
                    }
                }
            }
        }

        foreach (ThingInput player in instance.inputs)
        {
            player.canControl = currentCharacter.input == player;
        }

        if (currentCharacter != null)
            currentCharacter.MyTurn();
    }

    public static void StartGame(bool showLevelIntro = true)
    {
        if (showLevelIntro)
            instance.levelIntroUI.gameObject.SetActive(true);
        else
        {
            General.DelayedFunctionSeconds(instance, () =>
            {
                SetNextCharacter(false);
                ControlNextCharacter();
            }, delaySeconds: instance.changeCharacterDelay);
        }
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

        // Set up the menus
        foreach (Menu menu in FindObjectsOfType<Menu>(true))
        {
            if (menu.previousOption == null)
                menu.previousOption = touchControls;
        }

        // Set up the camera
        if (gameMode == GameMode.Play)
            GameplayCamera.instance.CenterCamera(-1f, Vector3.up * TilemapManager.instance.tilemap.size.z);

        // Start the game
        StartGame(gameMode == GameMode.Play);
    }

    public static void ResetInputModule()
    {
        if (instance == null)
            return;

        instance.inputModule.enabled = false;
        instance.inputModule.enabled = true;
    }

    private InputSystemUIInputModule inputModule
    {
        get
        {
            if (_inputModule == null)
                _inputModule = GetComponentInChildren<InputSystemUIInputModule>();
            return _inputModule;
        }
    }
    private InputSystemUIInputModule _inputModule;

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

    [SerializeField] private GameObject playerPrefab;

    public void SpawnPlayer(PlayerAndCharacter player, Vector3? position = null, PlayerSpawner playerSpawner = null)
    {
        if (playerInputManager == null)
            return;

        if (playerSpawner == null && position == null)
        {
            // Get all player spawners
            PlayerSpawner[] playerSpawners = FindObjectsOfType<PlayerSpawner>();

            // Choose a random player spawner
            if (playerSpawners.Length > 0)
                playerSpawner = playerSpawners[Random.Range(0, playerSpawners.Length)];
        }

        if (Instantiate(playerPrefab).TryGetComponent(out ThingInput input))
        // if (playerInputManager.JoinPlayer(-1, -1, null, player.player).TryGetComponent(out ThingInput input))
        {
            // Set the player's input
            ThingInput inputToUse = inputs.Find(x => x.playerInput != null && x.playerInput.devices.Count > 0 && x.playerInput.devices[0] == player.player);

            if (inputToUse == null)
            {
                inputToUse = input;
                inputToUse.playerInput.SwitchCurrentControlScheme(player.player);
            }
            else if (inputToUse != input)
                Destroy(input.gameObject);

            // Create the character
            if (Instantiate(characterPrefab).TryGetComponent(out CharacterThing character))
            {
                // Move the character
                if (position != null)
                    General.DelayedFunctionFrames(character, () =>
                    {
                        character.position = (Vector3)(Nodes.instance.gridGraph.GetNearest(position.Value).node.position);
                        Debug.Log("Spawned player at " + character.transform.position);
                    });
                else
                    General.DelayedFunctionFrames(character, () =>
                    {
                        character.position = playerSpawner.position;
                        Debug.Log("Spawned player at " + character.transform.position);
                    });

                character.characterInfo = player.character;

                // Add the character to the player's input
                input.inventory.AddThing(character, true, null, false);
                character.input = inputToUse;
            }
        }
    }

    [SerializeField] private GameObject cpuPlayerPrefab;

    public void SpawnCPUPlayer(int team)
    {

    }

    public void SpawnPlayers()
    {
        PlayerSpawner[] playerSpawners = FindObjectsOfType<PlayerSpawner>();
        List<PlayerSpawner> playerSpawnersList = new List<PlayerSpawner>(playerSpawners);

        // Create the players
        foreach (PlayerAndCharacter player in players)
        {
            if (playerSpawners.Length > 0)
            {
                if (playerSpawnersList.Count > 0)
                {
                    // Get a random player spawner
                    PlayerSpawner playerSpawner = playerSpawnersList[Random.Range(0, playerSpawnersList.Count)];

                    // Spawn the player
                    SpawnPlayer(player, null, playerSpawner);

                    // Remove the player spawner from the list
                    playerSpawnersList.Remove(playerSpawner);
                }
            }
            else
            {
                // If there are no player spawners, spawn the player at the world position of cell (0, 0) in the tilemap
                SpawnPlayer(player, Vector3.zero);
            }
        }
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        // 
    }
}
