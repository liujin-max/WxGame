using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 再生弹珠
    /// </summary>
    /// 
    public class BallRegen : Ball
    {
        private int m_Rate = 50;

        public override string GetDescription()
        {
            var str = string.Format("击中宝石时有概率使宝石恢复血量");

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.TriggerEnter(collision);
            this.OnHitElement(collision);
   
            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null) {
                if (RandomUtility.IsHit(30)) {
                    var value = (int)this.Demage.ToNumber();
                    //治疗特效
                    GameFacade.Instance.EffectManager.Load(EFFECT.HEALONCE, Vector3.zero, obt.gameObject);

                    obt.SetHP(obt.HP + value);
                    obt.OnShake();

                    GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONHITOBSTACLE, obt, value, this));

                } else {
                    this.OnHitObstable(collision);
                }
            }
        }
    }
}