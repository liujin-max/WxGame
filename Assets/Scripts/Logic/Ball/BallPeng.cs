using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 碰碰弹珠 每击落一颗弹珠，伤害增加#
    /// </summary>
    /// 
    public class BallPeng : Ball
    {
        private float m_Pow;
        private float m_Add = 0;


        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Pow = 1; //0.3f + level * 0.05f;
        }


        public override string GetDescription()
        {
            var str = string.Format("每击落一枚<sprite=0>，伤害增加<size=32><#43A600>{0}</color></size>点", m_Pow);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitBox(collision);
            this.OnHitObstable(collision);

            Ghost ghost = collision.transform.GetComponent<Ghost>();
            if (ghost != null) {
                if (ghost.IsDead() == true) {
                    m_Add += m_Pow;

                    m_Demage.PutADD(this, (int)m_Add);

                    GameFacade.Instance.EffectManager.Load(EFFECT.HITGROWTH, transform.localPosition);
                }
            }
        }
    }
}