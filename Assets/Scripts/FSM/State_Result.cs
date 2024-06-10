using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一步走完后的处理
public class State_Result<T> : State<Field>
{
    private _C.RESULT m_Result;
    private CDTimer m_Timer;
    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        m_Result = (_C.RESULT)values[0];

        m_Timer = new CDTimer(0.4f);
        
    }

    public override void Update()
    {
        if (m_Timer == null) return;

        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer = null;

            m_FSM.Owner.STATE   = _C.GAME_STATE.END;

            if (m_Result == _C.RESULT.VICTORY) {  //成功
                //结算奖励
                GameFacade.Instance.DataCenter.User.SetCoin(GameFacade.Instance.DataCenter.User.Coin + Field.Instance.Stage.Coin);

                GameFacade.Instance.UIManager.LoadWindow("VictoryWindow", UIManager.BOARD).GetComponent<VictoryWindow>();
            } else {    //失败
                GameFacade.Instance.UIManager.LoadWindow("LoseWindow", UIManager.BOARD).GetComponent<LoseWindow>();
            }
        }
    }
}

