using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 引力场 重力会不断波动，玩家需要及时调整策略以适应变化的环境。
    /// </summary>
    public class Gravity : Environment
    {
        private CDTimer m_Timer = new CDTimer(3f);

        public Gravity()
        {
            Name = "引力场";
        }

        public override string GetDescription()
        {
            return "场上的重力会不断波动。";
        }

        public override void OnLeave()
        {
            base.OnLeave();
            
            Physics2D.gravity = _C.DEFAULT_GRAVITY;
        }

        void Update()
        {
            m_Timer.Update(Time.deltaTime);
            if (m_Timer.IsFinished() == true) {
                m_Timer.Reset();

                Physics2D.gravity = new Vector2(0, -RandomUtility.Random(500, 1600) / 100.0f);
                Debug.Log("当前重力：" + Physics2D.gravity);
                //需要特效
            }
        }
    }
}