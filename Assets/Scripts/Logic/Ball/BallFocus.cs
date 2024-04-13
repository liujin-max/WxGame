using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //追踪
    public class BallFocus : Ball
    {
        private int m_Rate = 45;


        public override string GetDescription()
        {
            var str = "击中宝石后一定概率朝场上的<sprite=0>飞去";

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
                if (GameFacade.Instance.Game.Ghosts.Count == 0) {
                    return;
                }

                if (RandomUtility.IsHit(m_Rate) == true)
                {
                    Ghost ghost = GameFacade.Instance.Game.Ghosts[0];
                    if (ghost != null) {
                        this.Velocity = Vector2.zero;

                        Vector2 force = ghost.transform.localPosition - transform.localPosition;
                        Vector2 normal= Vector3.Normalize(force);
                        Vector2 vec     = normal * 850;

                        c_rigidbody.AddForce(vec);
                    }
                } 
            }
        }
    }
}

