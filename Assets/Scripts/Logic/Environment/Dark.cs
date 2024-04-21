using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 黑暗 关卡内的宝石被全部隐藏了，击中后才会显示 
    /// </summary>
    public class Dark : Environment
    {
        public Dark()
        {
            Name = "黑暗";
        }

        public override string GetDescription()
        {
            return "宝石陷入黑暗之中，直至被弹珠击中。";
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                obstacle.Show(false);
            });
        }

        public override void OnLeave()
        {
            base.OnLeave();
            
            GameFacade.Instance.Game.Obstacles.ForEach(obstacle =>{
                obstacle.Show(true);
            });
        }
    }
}