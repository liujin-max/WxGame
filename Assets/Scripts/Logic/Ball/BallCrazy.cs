using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 抽风弹珠
    /// </summary>
    /// 
    public class BallCrazy : Ball
    {
        private int m_Rate = 65;

        public override string GetDescription()
        {
            var str = string.Format("击中宝石后朝随机方向反弹");

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            
            bool flag1 = this.OnHitElement(collision);
            bool flag2 = this.OnHitObstable(collision);

            if (flag1 || flag2) {
                if (RandomUtility.IsHit(m_Rate) == true) {
                    Vector2 direction = Quaternion.Euler(0, 0, RandomUtility.Random(0, 360)) * Vector2.right;
                    this.Crash(direction * Velocity.magnitude);
                }
            }
        }
    }
}

