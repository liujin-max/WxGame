using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    public enum WallType
    {
        Normal,
        Top
    }



    public class Wall : MonoBehaviour
    {
        [EnumMultiAttribute]
        public WallType WallType;
    }
}

