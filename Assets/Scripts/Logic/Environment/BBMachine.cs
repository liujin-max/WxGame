using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 部分宝石具有护盾，需要多次击中才能消除
    /// </summary>
    public class BBMachine : Environment
    {
        private CDTimer m_Timer = new CDTimer(3f);

        public BBMachine()
        {
            Name = "磁力护盾";
        }

        public override string GetDescription()
        {
            return "部分宝石被护盾包围，需要多次击中才能消除。";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                if (RandomUtility.IsHit(30) == true) {
                    obstacle.AddShield(5);
                }
            });
        }

        public override void OnLeave()
        {
            base.OnLeave();
            
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                obstacle.RemoveShield();
            });
        }
    }
}