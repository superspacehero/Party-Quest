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

    public float changeCharacterDelay = 1f;

    #region Players

        public static List<ThingInput> players = new List<ThingInput>();
        public static void AddPlayer(ThingInput player)
        {
            players.Add(player);
        }

        public static void RemovePlayer(ThingInput player)
        {
            players.Remove(player);
        }

        public static void SetAllPlayersCanControl(bool canControl)
        {
            foreach (ThingInput player in players)
                player.canControl = canControl;
        }

    #endregion

    #region Things

        public static List<GameThing> things = new List<GameThing>();

        #region Characters
        
            [SerializeField] private ThingDisplay nextCharacterUI;
            public static List<CharacterThing> characters = new List<CharacterThing>();
            public static int currentCharacterIndex = 0;

            public CharacterPartList characterPartList;

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
                General.DelayedFunctionSeconds(instance, SetNextCharacter, delaySeconds:instance.changeCharacterDelay);
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

                foreach (ThingInput player in players)
                    player.canControl = player.GetAttachedThing() == currentCharacter;
            }

        #endregion

    #endregion

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // NextCharacter();
    }
}
