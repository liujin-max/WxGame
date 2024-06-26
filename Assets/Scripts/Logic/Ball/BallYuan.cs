using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 圆形弹珠 击中宝石后有概率把当前宝石转换成圆形
    /// </summary>
    /// 
    public class BallYuan : Ball
    {
        private int m_Rate;
        private int m_Order = 3;


        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Rate = 30 + 5 * m_Level;
        }



        public override string GetDescription()
        {
            var str = string.Format("击中宝石后有概率将当前宝石转换成<sprite=4>");

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null && obt.HasShield() == false) {
                if (obt.IsDead() == false && obt.Order != m_Order) {
                    if (RandomUtility.IsHit(m_Rate) == true) {
                        GameFacade.Instance.EffectManager.Load(EFFECT.SMOKE, obt.transform.localPosition);
                        GameFacade.Instance.SoundManager.Load(SOUND.EXCHANGE);
                        
                        GameFacade.Instance.Game.PushObstacle(obt.transform.localPosition, obt.HP, m_Order);

                        obt.ForceDead();
                    }
                }
            }
        }
    }
}