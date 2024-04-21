using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //蓄力弹珠
    public class BallPower : Ball
    {
        private int m_Count = 0;

        public override void Init(_C.BALLTYPE type)
        {
            base.Init(type);

            GameFacade.Instance.EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        public override string GetDescription()
        {
            return string.Format("回合结束时若留在弹珠槽中，则伤害提高1点。");
        }


        void OnReponsePlayEnd(GameEvent gameEvent)
        {
            //需要特效
            m_Count++;

            Demage.PutADD(this, m_Count);
        }
    }
}

