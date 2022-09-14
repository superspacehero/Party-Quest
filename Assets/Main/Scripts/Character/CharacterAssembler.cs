using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssembler : MonoBehaviour
{
    public class CharacterPart
    {
        public enum PartType
        {
            Head,
            Headgear,
            Hair,
            Eyebrow,
            Eye,
            Pupil,
            Nose,
            Ear,
            Earring,
            Mouth,
            Mustache,
            Beard,

            Body,
            Backpack,
            Necklace,
            Shirt,
            Jacket,
            Belt,
            Pants,

            Arm,
            Hand,
            Item,
            Glove,
            Ring,
            Bracelet,

            Leg,
            Footwear
        }

        public PartType partType;
        
    }
}
