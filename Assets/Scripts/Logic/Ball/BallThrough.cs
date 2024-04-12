using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //穿透 有一定的概率穿透宝石，每穿透1个宝石，伤害提高#倍
    public class BallThrough : Ball
    {
        private int m_Count;    //可追踪次数
        private int m_Current = 0;

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            m_Current = m_Count;
        }
        
        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Count = level * 2;
        }

        public override string GetDescription()
        {
            var str = string.Format("穿透宝石", m_Count);

            return str;
        }


        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            
            this.OnHitGhost(collision);
            bool flag = this.OnHitObstable(collision);



            //碰撞的对象是宝石
            if (flag == true) {
                // Physics2D.IgnoreCollision(c_collision, collision.collider, true);
                
            }
        }
    }
}

