using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //黑洞弹珠
    public class BallBlackHole : Ball
    {
        public override string GetDescription()
        {
            return "击中宝石后有低概率形成黑洞，吸附并销毁周围的宝石";
        }


        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            
            var obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null)
            {
                if (RandomUtility.IsHit(10))
                {
                    this.Dead();
                
                    //创建漩涡
                    GameFacade.Instance.Game.PushElement("Prefab/Box/BlackHole", obt.transform.localPosition);
                }
            }
        }
    }
}

