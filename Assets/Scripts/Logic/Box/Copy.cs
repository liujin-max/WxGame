using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace CB
{
    public class Copy : Box
    {
        void Start()
        {
            DoScale();
        }

        public override void OnHit(Ball ball,  int demage = 1)
        {
            if (this.IsDead() == true) return;
            if (ball == null || ball.IsSimulate || ball.IsCopy) return;

            m_HP--;

            if (this.IsDead()) {
                GameFacade.Instance.SoundManager.Load(SOUND.DROP);

                var copy = GameFacade.Instance.Game.PushBall(transform.localPosition, ball.Type);
                copy.IsCopy = true;
                copy.Crash(ball.Velocity * -1);
            }
        }
    }
}