using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 风力 
    /// </summary>
    public class Wind : Environment
    {
        private float m_windStrength = 2.5f;
        private Vector2 m_Direction;
        private float m_Angle;
        private CDTimer m_Timer = new CDTimer(10f);
        private GameObject m_Effect = null;

        public Wind()
        {
            Name = "沙尘";
        }

        public override string GetDescription()
        {
            return "弹珠可能会受到沙尘影响而改变轨迹。";
        }

        void GenerateAngles()
        {
            if (RandomUtility.IsHit(50)) {
                m_Angle = RandomUtility.Random(-80, 80);
            } else {
                m_Angle = RandomUtility.Random(100, 260);
            }

            m_Direction = new Vector2(Mathf.Cos(m_Angle * Mathf.Deg2Rad), Mathf.Sin(m_Angle * Mathf.Deg2Rad)).normalized * m_windStrength;
            Debug.Log("向量：" + m_Direction);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            GenerateAngles();

            m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.SANDSOTRM, Vector3.zero);
        }

        public override void OnLeave()
        {
            base.OnLeave();

            //销毁特效
            Destroy(m_Effect);
        }

        void Update()
        {
            m_Timer.Update(Time.deltaTime);
            if (m_Timer.IsFinished() == true)
            {
                m_Timer.Reset();

                GenerateAngles();
            }
        }
        
        void FixedUpdate()
        {
            if (m_Effect != null)
            {
                m_Effect.transform.localEulerAngles = new Vector3(0, 0, m_Angle);
            }

            GameFacade.Instance.Game.ActingBalls().ForEach(b => {
                b.AddForce(m_Direction);
            });
        }
    }
}