using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
public class VfxDisable : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => false;

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        gameObject.SetActive(false);
    }
}
