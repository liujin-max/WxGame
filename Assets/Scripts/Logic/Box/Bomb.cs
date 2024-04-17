using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    public class Bomb : Box
    {
        public Bomb()
        {
            m_HP    = 3;
        }

        void Start()
        {
            DoScale();
        }

        public override void OnHit(Ball ball,  int demage = 1)
        {
            if (this.IsDead() == true) return;
            
            m_HP -= demage;
        }

        public void Boom()
        {
            //产生爆炸
            GameFacade.Instance.EffectManager.Load(EFFECT.BOMBBOOM, transform.localPosition);

            //对范围内的障碍物造成伤害
            var radius = 2.5f;
            Vector3 collision_point = transform.localPosition;

            //伤害随层数成长
            int demage = (int)(GameFacade.Instance.Game.Stage * 1.5f);

            GameFacade.Instance.Game.Obstacles.ForEach(obt => {
                if (Vector3.Distance(obt.transform.localPosition, collision_point) <= radius) {
                    obt.OnHit(null, demage);
                }
            });

            //对box同样造成伤害
            GameFacade.Instance.Game.Boxs.ForEach(b => {
                if (Vector3.Distance(b.transform.localPosition, collision_point) <= radius && b.gameObject != gameObject) {
                    b.OnHit(null, demage);
                    b.OnShake();
                }
            });
        }

        public override void DoDead()
        {
            base.DoDead();

            Boom();
        }
    }
}