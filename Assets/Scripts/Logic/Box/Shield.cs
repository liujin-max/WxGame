using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    /// <summary>
    /// 护盾，受击N次后破裂
    /// </summary>
    public class Shield : Box
    {
        private SpriteRenderer m_Sprite;

        void Awake()
        {
            m_Sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
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


        public override void OnHit(Ball ball, int demage = 1)
        {
            if (this.IsDead() == true) return;
            if (ball != null && ball.IsSimulate == true) return;

            m_HP -= 1;

            OnShake();

            GameFacade.Instance.SoundManager.Load(SOUND.HITSHIELD);
    
        }

        public override void DoDead()
        {
            base.DoDead();

            GameFacade.Instance.EffectManager.Load(EFFECT.SHIELDBOOM, transform.parent.transform.localPosition);
        }
    }
}

