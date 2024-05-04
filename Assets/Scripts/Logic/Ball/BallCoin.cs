using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;


namespace CB
{
    //黄金弹珠
    public class BallCoin : Ball
    {
        private int m_Coin = 2;

        public override void Init(_C.BALLTYPE type)
        {
            base.Init(type);

            EventManager.AddHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.ONPLAYEND,        OnReponsePlayEnd);
        }

        public override string GetDescription()
        {
            return string.Format("回合结束时若留在发射槽中，则获得{0}<sprite={1}>。", m_Coin, (int)_C.SPRITEATLAS.COIN);
        }


        void OnReponsePlayEnd(GameEvent gameEvent)
        {
            GameFacade.Instance.Game.UpdateCoin(m_Coin);

            var item = GameFacade.Instance.Game.GameUI.GetBallSeat(this);
            if (item != null) {
                var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, item.transform.position);
                e.GetComponent<FlyCoin>().Fly(0, false); 
            }
            
        }
    }
}

