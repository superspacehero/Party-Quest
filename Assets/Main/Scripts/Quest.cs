using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    
    [System.Serializable]
    public struct Info
    {
        public string name;
        public Sprite icon;
        public string description;
    }
    public Info info;

    [System.Serializable]
    public struct Stat
    {
        public int currency;
        public List<Thing> things;
        public int exp;

        public Stat(int currency, List<Thing> things, int exp)
        {
            this.currency = currency;
            this.things = things;
            this.exp = exp;
        }
    }
    public Stat reward = new Stat(100, new List<Thing>(), 100);

}
