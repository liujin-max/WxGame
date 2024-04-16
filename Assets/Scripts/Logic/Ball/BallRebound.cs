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
        private int m_Count;
        private int m_Current;


        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            m_Current = m_Count;
            m_GroundValid = false;
        }

        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Count = 2 * m_Level;
        }

        public override string GetDescription()
        {
            var str = string.Format("触底后强力反弹,最多<size=32><#43A600>{0}</color></size>次", m_Count);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitBox(collision);
            this.OnHitObstable(collision);

            Ground ground = collision.transform.GetComponent<Ground>();
            if (ground != null) {
                if (ground.GroundType == GroundType.Ground) {
                    if (m_Current > 0) {
                        m_Current--;

                        Vector3 random_pos = new Vector3(RandomUtility.Random(-43, 44) / 10.0f, 6f, 0);
                        Vector2 force   = random_pos - transform.localPosition;
                        Vector2 normal  = Vector3.Normalize(force);
                        Vector2 vec     = normal * 850;

                        c_rigidbody.AddForce(vec);
                    }
                }
            }
        }

        public override void OnCollisionExit2D(Collision2D collision)
        {
            Ground ground = collision.transform.GetComponent<Ground>();
            if (ground != null) {
                if (ground.GroundType == GroundType.Ground) {
                    if (m_Current <= 0) {
                        m_GroundValid = true;
                    }
                }
            }  
        }
    }
}

