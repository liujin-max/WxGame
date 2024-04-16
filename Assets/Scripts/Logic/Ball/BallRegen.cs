using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 再生弹珠 击落宝石后有概率在原地产出一个新的宝石
    /// </summary>
    /// 
    public class BallRegen : Ball
    {
        private int m_Rate = 50;

        public override string GetDescription()
        {
            var str = string.Format("击落宝石后有概率在原地留下一颗新的宝石", m_Rate);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitBox(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null) {
                if (obt.IsDead() == true) {
                    if (RandomUtility.IsHit(m_Rate) == true) {
                        int turn    = GameFacade.Instance.Game.Stage;
                        int hp      = (int)Mathf.Ceil(RandomUtility.Random(turn * 60, turn * 140) / 100.0f);

                        var new_obt = GameFacade.Instance.Game.PushObstacle(obt.transform.localPosition, hp);
                        new_obt.DoScale();
                    }
                }
            }
        }
    }
}