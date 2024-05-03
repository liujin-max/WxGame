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
        public AttributeValue Radius = new AttributeValue(3f);

        void Awake()
        {
            m_HP    = 3;

            //伤害随层数成长
            Demage  = new AttributeValue((int)(GameFacade.Instance.Game.Stage * 1.5f));

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

            m_Sprite.transform.DOShakeRotation(0.25f, 25f, vibrato: 15, randomness: 50).OnComplete(()=>{
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
            GameFacade.Instance.EventManager.SendEvent(new GameEvent(EVENT.ONBOMBBEFORE, this));
            Platform.Instance.VIBRATE(_C.VIBRATELEVEL.HEAVY);

            //产生爆炸
            float radius = Radius.ToNumber();

            var scale = radius / Radius.GetBase();
            var obj = GameFacade.Instance.EffectManager.Load(EFFECT.BOMBBOOM, transform.localPosition);
            obj.transform.localScale = new Vector3(scale, scale, scale);

            //对范围内的障碍物造成伤害
            GameFacade.Instance.Game.Boom(transform.localPosition, radius, (int)Demage.ToNumber());
        }

        public override void DoDead()
        {
            base.DoDead();

            Boom();
        }
    }
}