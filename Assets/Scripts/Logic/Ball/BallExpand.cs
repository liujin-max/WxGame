using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 膨胀弹珠 击中菱形宝石后体积膨胀,最大膨胀到1.5倍
    /// </summary>
    /// 
    public class BallExpand : Ball
    {
        private float m_Rate = 0.3f;

        public override string GetDescription()
        {
            var str = string.Format("击中<sprite=5>后体积膨胀，击中其他宝石则恢复原状");

            return str;
        }

        void FlushScale()
        {
            float scale = m_Scale.ToNumber();
            transform.localScale = new Vector3(scale, scale, scale);
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitBox(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null && obt.HasShield() == false) {
                if (obt.Order == 2) {
                    m_Scale.PutADD(this, m_Rate);
                } else {
                    m_Scale.Pop(this);
                }
                
                FlushScale();
            }

            var ground = collision.transform.GetComponent<Ground>();
            if (ground != null && ground.GroundType == GroundType.Ground)
            {
                m_Scale.Pop(this);
                FlushScale();
            }
        }
    }
}