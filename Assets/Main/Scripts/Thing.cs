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

        [FoldoutGroup("Info")]
        public bool useLocalizedString = false;
        [FoldoutGroup("Info"), HideIf("useLocalizedString"), Tooltip("The name of this thing.")]
        public string nameString;
        [FoldoutGroup("Info"), ShowIf("useLocalizedString"), Tooltip("The description of this thing.")]
        public LocalizedString localizedNameString;
        public string name
        {
            get
            {
                if (useLocalizedString)
                    return localizedNameString;
                else
                    return nameString;
            }
        }

        [FoldoutGroup("Info")]
        public LocalizedString descriptionString;
        [FoldoutGroup("Info")]
        public Sprite icon;
        [FoldoutGroup("Info")]
        public int value;
        [FoldoutGroup("Info")]
        public Health health;

    #endregion

    #region Other Variables

        [SerializeField, FoldoutGroup("SFX"), Tooltip("The main sounds this things makes. Used for movement sounds by moving things, and usage sounds by items.")]
        protected SFX mainSFX;

    #endregion

    #region SFX

        public void PlayMainSFX()
        {
            mainSFX.Play();
        }

    #endregion
}
