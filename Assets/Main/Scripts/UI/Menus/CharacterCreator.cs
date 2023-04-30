using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using I2.Loc;

[RequireComponent(typeof(CharacterUI))]
public class CharacterCreator : GameThing
{
    #region Fields
    
    #endregion

    #region Properties
    #endregion

    #region Methods

        #region Input
            private Vector2Int inputDirection = Vector2Int.zero;

            public override void Move(Vector2 direction)
            {
                base.Move(direction);
            }

            public override void PrimaryAction(bool value)
            {
                base.PrimaryAction(value);
            }
            
            public override void SecondaryAction(bool value)
            {
                base.SecondaryAction(value);
            }
        #endregion
    #endregion
}
