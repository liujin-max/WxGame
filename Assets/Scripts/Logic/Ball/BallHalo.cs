using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 光环弹珠
    /// </summary>
    /// 
    public class BallHalo : Ball
    {
        private int m_Power;
        private HashSet<Ball> m_Halos = new HashSet<Ball>();

        public override void Init(_C.BALLTYPE type)
        {
            base.Init(type);

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLSHOOT,      OnBallShoot);
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONBALLRECYCLE,    OnBallRecycle);
        }

        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Power = m_Level;

            m_Demage.SetBase(1);
        }

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            m_Halos.Clear();
            foreach (var ball in GameFacade.Instance.Game.m_Balls) {
                if (ball.GetState() == (int)_C.LAYER.BALLACTING && ball != this) {
                    ball.m_Demage.PutADD(this, m_Power);
                    m_Halos.Add(ball);
                }
            }
        }

        public override void Recyle()
        {
            base.Recyle();

            foreach (var ball in m_Halos) {
                ball.m_Demage.Pop(this);
            }
            m_Halos.Clear();
        }

        public override string GetDescription()
        {
            var str = string.Format("伤害不随等级成长。发射途中为场上其他弹珠提供<size=32><#43A600>{0}</color></size>点伤害", m_Power);

            return str;
        }

        void OnBallShoot(GameEvent gameEvent)
        {
            if (this.GetState() != (int)_C.LAYER.BALLACTING) return;

            var ball = (Ball)gameEvent.GetParam(0);
            if (ball != this) {
                ball.m_Demage.PutADD(this, m_Power);
                m_Halos.Add(ball);
            }
        }

        void OnBallRecycle(GameEvent gameEvent)
        {
            if (this.GetState() != (int)_C.LAYER.BALLACTING) return;

            var ball = (Ball)gameEvent.GetParam(0);
            if (m_Halos.Contains(ball) == true) {
                m_Halos.Remove(ball);
                ball.m_Demage.Pop(this);
            }
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLSHOOT,      OnBallShoot);
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONBALLRECYCLE,    OnBallRecycle);
        }
    }
}