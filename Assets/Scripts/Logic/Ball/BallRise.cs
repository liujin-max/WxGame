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
        private int m_Rate = 65;    //几率


        public override string GetDescription()
        {
            var str = string.Format("击落宝石有概率在原地留下一枚<sprite=0>。", m_Rate);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null) {
                if (obt.IsDead() == true) {
                    if (RandomUtility.IsHit(m_Rate) == true) {
                        GameFacade.Instance.Game.PushGhost(collision.transform.localPosition);
                    }
                }
            }
        }
    }
}
