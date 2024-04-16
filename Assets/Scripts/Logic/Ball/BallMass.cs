using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 重力弹珠(碰撞产生范围伤害)
    /// </summary>
    public class BallMass : Ball
    {
        public override string GetDescription()
        {
            var str = string.Format("重量比其他弹珠更大，弹力也更大。");

            return str;
        }
    }
}

