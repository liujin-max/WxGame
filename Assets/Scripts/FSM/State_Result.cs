using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一步走完后的处理
public class State_Result<T> : State<Field>
{
    private CDTimer m_Timer;
    private _C.RESULT m_Result;
    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        m_Result = (_C.RESULT)values[0];

        m_Timer = new CDTimer(1.0f);

        //子弹时间
        GameFacade.Instance.TimeManager.BulletTime();
    }

    public override void Update()
    {
        if (!GameFacade.Instance.DisplayEngine.IsIdle() == true) return;
    
        if (m_Timer == null) return;
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished()) {
            m_Timer = null;

            Time.timeScale = 1f;
            Field.Instance.STATE   = _C.GAME_STATE.END;

            //关卡模式
            if (Field.Instance.Stage.MODE == _C.MODE.CHAPTER)
            {
                if (m_Result == _C.RESULT.VICTORY) {  //成功
                    //记录关卡序列
                    GameFacade.Instance.DataCenter.User.SetLevel(Field.Instance.Stage.ID);
                    //结算奖励
                    GameFacade.Instance.DataCenter.User.UpdateCoin(Field.Instance.Stage.Coin);
                    GameFacade.Instance.DataCenter.User.UpdateFood(Field.Instance.Stage.Food);

                    EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECOIN));

                    var window = GameFacade.Instance.UIManager.LoadWindow("VictoryWindow", UIManager.BOARD).GetComponent<VictoryWindow>();
                    window.Init();

                } else {    //失败
                    var window = GameFacade.Instance.UIManager.LoadWindow("LoseWindow", UIManager.BOARD).GetComponent<LoseWindow>();
                    window.Init();
                }
            }
            
        }
    }
}

