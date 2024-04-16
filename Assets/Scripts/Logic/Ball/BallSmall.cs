using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    public class BallSmall : Ball
    {
        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            
            bool flag1 = this.OnHitBox(collision);
            bool flag2 = this.OnHitObstable(collision);

            if (flag1 == true || flag2 == true) {
                this.Dead();
            }
        }
    }
}