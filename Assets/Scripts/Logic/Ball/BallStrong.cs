using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 强力弹珠(对障碍物造成更高的伤害)
    /// </summary>
    public class BallStrong : Ball
    {
        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);
            m_Demage.SetBase(2 * m_Level);
        }
        
        public override string GetDescription()
        {
            var str = string.Format("每次撞击可以造成<size=32><#43A600>{0}</color></size>点伤害", m_Demage.ToNumber());

            return str;
        }

    }
}