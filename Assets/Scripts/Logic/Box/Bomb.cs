using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    public class Bomb : Box
    {
        private SpriteRenderer m_Sprite;
        public AttributeValue Demage;
        public AttributeValue Radius = new AttributeValue(2.5f);

        public Bomb()
        {
            m_HP    = 3;

            //伤害随层数成长
            Demage  = new AttributeValue((int)(GameFacade.Instance.Game.Stage * 1.5f));
        }

        void Awake()
        {
            m_Sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            DoScale();
        }

        public override void OnShake()
        {
            if (m_Shaking == true) {
                return;
            }
            
            m_Shaking = true;

            m_Sprite.transform.DOShakeRotation(0.25f, 20f, vibrato: 15, randomness: 50).OnComplete(()=>{
                m_Shaking = false;
            });
        }

        public override void OnHit(Ball ball,  int demage = 1)
        {
            if (this.IsDead() == true) return;
            
            m_HP -= demage;

            OnShake();
        }

        public void Boom()
        {
            //产生爆炸
            GameFacade.Instance.EffectManager.Load(EFFECT.BOMBBOOM, transform.localPosition);

            //对范围内的障碍物造成伤害
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBOMBBEFORE, this));

            GameFacade.Instance.Game.Boom(transform.localPosition, Radius.ToNumber(), (int)Demage.ToNumber());
        }

        public override void DoDead()
        {
            base.DoDead();

            Boom();
        }
    }
}