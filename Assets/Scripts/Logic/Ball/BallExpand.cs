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
        private float m_Rate;
        private float m_Add = 0.1f;
        private int m_Count;


        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Rate = Math.Min(2f, 1.3f + 0.1f * (level - 1));
        }



        public override string GetDescription()
        {
            var str = string.Format("击中<sprite=5>后体积膨胀{0}倍(最大<size=32><#43A600>{1}</color></size>倍)，击中其他宝石则缩小(最小为原始大小)", m_Add, m_Rate);

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
            this.OnHitGhost(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null && obt.HasShield() == false) {
                if (obt.Order == 2) {
                    m_Count++;
                } else {
                    m_Count = (int)MathF.Max(0, m_Count - 1);
                }

                float add_value = m_Count * m_Add;
                add_value   = Math.Min(add_value, m_Rate - 1.0f);
                m_Scale.PutADD(this, add_value);
                FlushScale();
            }

            var ground = collision.transform.GetComponent<Ground>();
            if (ground != null && ground.GroundType == GroundType.Ground)
            {
                m_Count = 0;

                m_Scale.Pop(this);
                FlushScale();
            }
        }
    }
}