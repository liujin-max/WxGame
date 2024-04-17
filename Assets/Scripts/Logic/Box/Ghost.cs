using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    public class Ghost : Box
    {
        public string Name { get {return "碎片";}}
        public string Icon { get {return "UI/Texture/Glass";}}

        void Start()
        {
            DoScale();
        }

        public override void OnHit(Ball ball,  int demage = 1)
        {
            if (this.IsDead() == true) return;
            
            m_HP -= demage;
        }

        public override void DoDead()
        {
            GameFacade.Instance.SoundManager.Load(SOUND.HITGHOST);
            GameFacade.Instance.EffectManager.Load(EFFECT.FLYGLASS, transform.localPosition);

            GameFacade.Instance.Game.PushGlass(1);   

            Destroy(gameObject);
        }
    }
}