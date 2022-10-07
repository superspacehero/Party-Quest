using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;

public class Thing : LookAtObject
{
    // This class is used as the basis for all objects in the game.
    // It contains the basic properties of an object, such as its localized name,
    // its localized description, its icon, and its value.

    #region Info

        #region Name

            [FoldoutGroup("Info")]
            public bool useLocalizedString = false;
            [FoldoutGroup("Info"), HideIf("useLocalizedString"), Tooltip("The name of this thing.")]
            public string nameString;
            [FoldoutGroup("Info"), ShowIf("useLocalizedString"), Tooltip("The description of this thing.")]
            public LocalizedString localizedNameString;

            public bool properNoun = false;

            new public string name
            {
                get
                {
                    if (useLocalizedString)
                        return localizedNameString;
                    else
                        return nameString;
                }
            }

        #endregion

        [FoldoutGroup("Info")]
        public LocalizedString descriptionString;
        [FoldoutGroup("Info")]
        public Sprite icon;
        [FoldoutGroup("Info")]
        public int value;
        [FoldoutGroup("Info")]
        public Health health;

    #endregion

    #region Rooms

        [FoldoutGroup("Rooms")]
        public List<Level.Room> rooms = new List<Level.Room>();

        public void WhatRoomsAmIIn(bool deactivateIfRoomsNotDiscovered)
        {
            rooms.Clear();

            if (GameManager.instance.level.rooms.Count <= 0)
                return;

            foreach (Level.Room room in GameManager.instance.level.rooms)
            {
                if (room.Contains(this))
                    rooms.Add(room);
            }

            if (rooms.Count > 0)
            {
                foreach (Level.Room room in rooms)
                {
                    if (!room.GetDiscovered() && deactivateIfRoomsNotDiscovered)
                    {
                        gameObject.SetActive(false);
                        return;
                    }
                }
            }
            else
            {
                // Debug.LogError(name + " is not in any rooms!", this);
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            WhatRoomsAmIIn(deactivateIfRoomsNotDiscovered:true);
        }

    #endregion

    #region SFX

        [SerializeField, FoldoutGroup("SFX"), Tooltip("The main sounds this things makes. Used for movement sounds by moving things, and usage sounds by items.")]
        protected SFX mainSFX;

        public void PlayMainSFX()
        {
            mainSFX.Play();
        }

    #endregion
}
