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
        private int m_Rate;
    


        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);
        }

        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);
            m_Rate = 30 + 5 * m_Level;
        }

        public override string GetDescription()
        {
            var str = string.Format("击中宝石后有<size=32><#43A600>{0}%</color></size>概率朝随机方向反弹", m_Rate);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            
            bool flag1 = this.OnHitGhost(collision);
            bool flag2 = this.OnHitObstable(collision);

            if (flag1 || flag2) {
                if (RandomUtility.IsHit(m_Rate) == true) {
                    Vector2 direction = Quaternion.Euler(0, 0, RandomUtility.Random(0, 360)) * Vector2.right;
                    this.Crash(direction * 10);
                }
            }
        }
    }
}

