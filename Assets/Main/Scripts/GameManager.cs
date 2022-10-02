using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviour
{
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

    public Level level = new Level();

    public float changeCharacterDelay = 1f;

    #region Players

        public static List<Player> players = new List<Player>();
        public static void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public static void RemovePlayer(Player player)
        {
            players.Remove(player);
        }

        public static void SetAllPlayersCanControl(bool canControl)
        {
            foreach (Player player in players)
                player.canControl = canControl;
        }

    #endregion

    #region Characters
    
        [SerializeField] private CharacterUI nextCharacterUI;
        public static List<Character> characters = new List<Character>();
        public static int currentCharacterIndex = 0;

        public static Character currentCharacter
        {
            get
            {
                if (characters.Count <= 0)
                    return null;

                return charactersInCurrentTeam[currentCharacterIndex];
            }
        }

        public static void AddCharacter(Character character)
        {
            characters.Add(character);
        }
        
        public static void RemoveCharacter(Character character)
        {
            characters.Remove(character);
        }

        #region Teams

            public static List<int> teams = new List<int>();
            public static List<Character> charactersInCurrentTeam = new List<Character>();

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
            General.DelayedFunctionSeconds(instance, SetNextCharacter, delaySeconds:instance.changeCharacterDelay);
        }
        private static void SetNextCharacter()
        {
            if (characters.Count == 0)
                return;

            currentCharacterIndex++;
            if (currentCharacterIndex >= charactersInCurrentTeam.Count)
                currentTeamIndex++;

            instance.nextCharacterUI.gameObject.SetActive(true);

            foreach (Player player in players)
            {
                if (player.controlledThing == charactersInCurrentTeam[currentCharacterIndex])
                    player.SetControlObject();
            }
        }

    #endregion

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // NextCharacter();
    }
}
