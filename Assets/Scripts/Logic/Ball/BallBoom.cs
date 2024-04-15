using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 爆炸弹珠(碰撞产生范围伤害)
    /// </summary>
    public class BallBoom : Ball
    {
        private float m_Radius = 180;

        public override string GetDescription()
        {
            var str = string.Format("击中宝石时发生爆炸，造成范围伤害。");

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitGhost(collision);

            //
            if (collision.gameObject.GetComponent<Obstacle>() != null || collision.gameObject.GetComponent<Ghost>() != null) {
                if (collision.contacts.Length <= 0){
                    return;
                }

                Vector3 collision_point = collision.contacts[0].point;

                float radius    = this.m_Radius / 100.0f;

                //对范围内的障碍物造成伤害
                var obstacles   = GameFacade.Instance.Game.Obstacles;
                for (int i = 0; i < obstacles.Count; i++) {
                    Obstacle obt = obstacles[i]; 
                    if (Vector3.Distance(obt.transform.localPosition, collision_point) <= radius) {
                        obt.OnHit(this, (int)m_Demage.ToNumber());
                    }
                }

                //对ghost同样造成伤害
                var ghosts   = GameFacade.Instance.Game.Ghosts;
                for (int i = 0; i < ghosts.Count; i++) {
                    Ghost ghost = ghosts[i];
                    if (Vector3.Distance(ghost.transform.localPosition, collision_point) <= radius && ghost.gameObject != collision.gameObject) {
                        ghost.OnHit(this, (int)m_Demage.ToNumber());
                        ghost.OnShake();
                    }

                }


                float rate = this.m_Radius / 100f;
                var obj = GameFacade.Instance.EffectManager.Load(EFFECT.BOOM, collision_point);
                obj.transform.localScale = new Vector3(rate, rate, rate);
            }
        }
    }
}

