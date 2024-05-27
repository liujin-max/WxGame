using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 弹力弹珠
    /// </summary>
    /// 
    public class BallRebound : Ball
    {
        private int m_Rate = 85;

        public override string GetDescription()
        {
            return string.Format("触底后有概率强力反弹。");
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
            this.OnHitObstable(collision);

            Ground ground = collision.transform.GetComponent<Ground>();
            if (ground != null) {
                if (ground.GroundType == GroundType.Ground) {
                    if (RandomUtility.IsHit(m_Rate)) {
                        m_Rate = Math.Max(0, m_Rate - 15);
                        m_GroundValid = false;

                        Vector3 random_pos = new Vector3(RandomUtility.Random(-43, 44) / 10.0f, 6f, 0);
                        Vector2 force   = random_pos - transform.localPosition;
                        Vector2 normal  = Vector3.Normalize(force);
                        Vector2 vec     = normal * 850;

                        c_rigidbody.AddForce(vec);
                    } else {
                        m_GroundValid = true;
                    }
                }
            }
        }
    }
}

