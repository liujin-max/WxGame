using System;
using UnityEngine;

namespace CB
{
    /// <summary>
    /// 高速弹珠
    /// </summary>
    /// 
    public class BallSpeed : Ball
    {
        private float m_Rate = 1;
        public override string GetDescription()
        {
            var str = "发射后，弹珠的速度会随时间推移而增加";

            return str;
        }

        void Update()
        {
            if (this.IsActing == true) {
                m_Rate += Time.deltaTime;
            }
        }

        void FixedUpdate()
        {
            if (this.IsActing == false) return;


            var length = Math.Max(Velocity.magnitude, m_Rate);
            Velocity = Velocity.normalized * length;
        }
    }
}