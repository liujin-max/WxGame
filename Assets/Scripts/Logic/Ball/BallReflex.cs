using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //反射弹珠
    public class BallReflex : Ball
    {
        public override string GetDescription()
        {
            var str = "撞击墙壁后总能朝高处强力反弹";

            return str;
        }
    }
}

