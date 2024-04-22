using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace CB
{
    /// <summary>
    /// 
    /// </summary>
    public class NOCoin : Environment
    {
        public NOCoin()
        {
            Name = "颗粒无收";
        }

        public override string GetDescription()
        {
            return "本回合无法获得结算金币。";
        }

        void Awake()
        {
            GameFacade.Instance.EventManager.AddHandler(EVENT.ONWILLRECEIVECOIN,   OnReponseReceiveCoin);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.ONWILLRECEIVECOIN,   OnReponseReceiveCoin);
        }

        void OnReponseReceiveCoin(GameEvent gameEvent)
        {
            AttributeValue coin_number = (AttributeValue)gameEvent.GetParam(0);
            coin_number.PutMUL(this, 0);
        }
    }
}