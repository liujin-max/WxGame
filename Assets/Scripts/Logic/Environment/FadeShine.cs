using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 
    /// </summary>
    public class FadeShine : Environment
    {
        private CDTimer m_CDTimer = new CDTimer(0.4f);
        public FadeShine()
        {
            Name = "闪烁";
        }

        public override string GetDescription()
        {
            return "场上的宝石若隐若现。";
        }
        
        void Update()
        {
            if(m_IsEnter != true) return;


            m_CDTimer.Update(Time.deltaTime);
            if (m_CDTimer.IsFinished() == true)
            {
                m_CDTimer.Reset();

                var obstacles = new List<Obstacle>();

                GameFacade.Instance.Game.Obstacles.ForEach(o =>{
                    if (o.IsValid() == true) {
                        obstacles.Add(o);
                    }
                });

                if (obstacles.Count > 0)
                {
                    var rand = RandomUtility.Random(0, obstacles.Count);
                    obstacles[rand].Shine();
                }
            }
        }
    }
}