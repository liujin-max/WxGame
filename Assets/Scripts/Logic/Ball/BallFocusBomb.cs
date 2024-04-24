using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //炸弹
    public class BallFocusBomb : Ball
    {
        private int m_Rate = 45;


        public override string GetDescription()
        {
            return string.Format("击中宝石后有一定概率朝场上的<sprite={0}>飞去", (int)_C.SPRITEATLAS.BOMB);
        }


        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            
            this.OnHitElement(collision);
            bool flag = this.OnHitObstable(collision);

            //碰撞的对象是宝石
            if (flag == true) {
                if (GameFacade.Instance.Game.Boxs.Count == 0) {
                    return;
                }

                if (RandomUtility.IsHit(m_Rate) == true)
                {
                    for (int i = 0; i < GameFacade.Instance.Game.Boxs.Count; i++)
                    {
                        var b = GameFacade.Instance.Game.Boxs[i].GetComponent<Bomb>();
                        if (b != null) {
                            this.Velocity = Vector2.zero;

                            Vector2 force = b.transform.localPosition - transform.localPosition;
                            Vector2 normal= Vector3.Normalize(force);
                            Vector2 vec     = normal * 850;

                            c_rigidbody.AddForce(vec);
                            break;
                        }
                    }
                } 
            }
        }
    }
}

