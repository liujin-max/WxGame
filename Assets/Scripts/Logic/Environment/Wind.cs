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
        private GameObject m_Effect = null;

        public Wind()
        {
            Name = "沙尘";
        }

        public override string GetDescription()
        {
            return "弹珠可能会受到沙尘影响而改变轨迹。";
        }

        public override void OnEnter()
        {
            base.OnEnter();


            float angle = 0; // 角度值

            if (RandomUtility.IsHit(50)) {
                angle = RandomUtility.Random(-80, 80);
            } else {
                angle = RandomUtility.Random(100, 260);
            }

            m_Direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized * m_windStrength;
            Debug.Log("向量：" + m_Direction);


            m_Effect = GameFacade.Instance.EffectManager.Load(EFFECT.SANDSOTRM, Vector3.zero);
            m_Effect.transform.localEulerAngles = new Vector3(0, 0, angle);
        }

        public override void OnLeave()
        {
            base.OnLeave();

            //销毁特效
            Destroy(m_Effect);
        }

        
        void FixedUpdate()
        {
            GameFacade.Instance.Game.ActingBalls().ForEach(b => {
                b.AddForce(m_Direction);
            });
        }
    }
}