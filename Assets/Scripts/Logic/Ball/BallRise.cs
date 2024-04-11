using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 奇迹弹珠
    /// </summary>
    /// 
    public class BallRise : Ball
    {
        private int m_Need;
        private int m_Rate = 10;    //几率

        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Need = (int)m_Demage.ToNumber() * 3;
            m_Rate = Math.Min(60, 10 + 2 * m_Level);
        }

        public override string GetDescription()
        {
            var str = string.Format("当命中的宝石低于<size=32><#43A600>{0}</color></size>点血量时,有<size=32><#43A600>{1}%</color></size>的概率直接击落并在原地留下一枚<sprite=0>。", m_Need, m_Rate);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitGhost(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null) {
                if (obt.HP <= m_Need) {
                    if (RandomUtility.IsHit(m_Rate) == true) {
                        obt.OnHit(this, obt.HP);
                        GameFacade.Instance.Game.PushGhost(collision.transform.localPosition);
                    }
                }
            }
        }
    }
}
