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
        private SpriteRenderer m_Bubble;


        void Awake()
        {
            m_Bubble = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        }

        public override void OnShake()
        {
            if (m_Shaking == true) {
                return;
            }
            
            m_Shaking = true;

            m_Bubble.transform.DOShakeRotation(0.25f, 30f, vibrato: 15, randomness: 50).OnComplete(()=>{
                m_Shaking = false;
            });
        }


        public override void OnHit(Ball ball, int demage = 1)
        {
            if (this.IsDead() == true) return;
            if (ball.m_IsSimulate == true) return;

            m_HP -= 1;

            OnShake();

            GameFacade.Instance.SoundManager.Load(SOUND.HITSHIELD);
    
        }



        public override void Dispose()
        {
            GameFacade.Instance.EffectManager.Load(EFFECT.SHIELDBOOM, transform.parent.transform.localPosition);

            Destroy(gameObject);
        }
    }
}

