using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Scenery : Thing
{
    [FoldoutGroup("Info")]
    public List<Thing> items;
}
