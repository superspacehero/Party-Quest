using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Scenery : GridThing
{
    [FoldoutGroup("Info")]
    public List<GridThing> items;
}
