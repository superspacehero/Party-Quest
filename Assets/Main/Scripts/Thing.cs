using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;
using Sirenix.OdinInspector;

public class Thing : MonoBehaviour
{
    // This class is used as the basis for all objects in the game.
    // It contains the basic properties of an object, such as its localized name,
    // its localized description, its icon, and its value.

    #region Info

        #region Name

            [FoldoutGroup("Info/Name")]
            public bool useLocalizedString = false;
            [FoldoutGroup("Info/Name"), HideIf("useLocalizedString"), Tooltip("The name of this thing.")]
            public string nameString;
            [FoldoutGroup("Info/Name"), ShowIf("useLocalizedString"), Tooltip("The description of this thing.")]
            public LocalizedString localizedNameString;
            [FoldoutGroup("Info/Name")]
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

        [FoldoutGroup("Info")]
        public bool canDiscoverRooms = false;

    #endregion

    #region Rooms

        [FoldoutGroup("Rooms")]
        public List<Level.Room> rooms = new List<Level.Room>();

        public void WhatRoomsAmIIn(bool deactivateIfRoomsNotDiscovered, bool discoverRooms = false)
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
                        if (canDiscoverRooms)
                            room.SetDiscovered(true);
                        else
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
            General.DelayedFunctionFrames(this, () => WhatRoomsAmIIn(deactivateIfRoomsNotDiscovered:true, discoverRooms:canDiscoverRooms));
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
