using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //缩放弹珠 被击中的宝石概率会缩放
    public class BallScale : Ball
    {
        private int m_Rate = 35;

        public override string GetDescription()
        {
            var str = string.Format("有一定概率使被击中的宝石放大或缩小。");

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
                if (RandomUtility.IsHit(m_Rate) == true)
                {
                    float offset = 0.1f;
                    if (RandomUtility.IsHit(45) == true)
                    {
                        offset = -0.1f;
                    }

                    Obstacle obt = collision.gameObject.GetComponent<Obstacle>();
                    obt.m_Scale.PutADD(this, offset);
                    obt.JudgeScale();
                }
            }
        }
    }
}

