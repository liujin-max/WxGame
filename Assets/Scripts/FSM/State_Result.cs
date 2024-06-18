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

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));
    }

    public override void Update()
    {
        if (!GameFacade.Instance.DisplayEngine.IsIdle() == true) return;
    
        if (m_Timer == null) return;
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished()) {
            m_Timer = null;

            Field.Instance.STATE   = _C.GAME_STATE.END;

            EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));

            //关卡模式
            if (Field.Instance.Stage.MODE == _C.MODE.CHAPTER)
            {
                if (m_Result == _C.RESULT.VICTORY) {  //成功
                    //记录关卡序列
                    GameFacade.Instance.DataCenter.User.SetLevel(Field.Instance.Stage.ID);
                    //结算奖励
                    Field.Instance.Stage.ReceiveReward();

                    var window = GameFacade.Instance.UIManager.LoadWindow("VictoryWindow", UIManager.BOARD).GetComponent<VictoryWindow>();
                    window.Init();

                } else {    //失败
                    var window = GameFacade.Instance.UIManager.LoadWindow("LoseWindow", UIManager.BOARD).GetComponent<LoseWindow>();
                    window.Init();
                }
            }
            else if (Field.Instance.Stage.MODE == _C.MODE.ENDLESS)
            {
                //记录分数
                GameFacade.Instance.DataCenter.User.SetScore(Field.Instance.Stage.GetScore());
                //结算奖励
                Field.Instance.Stage.ReceiveReward();

                //任务：每日挑战
                GameFacade.Instance.DataCenter.Daily.FinishTask((int)_C.TASK.ENDLESS);


                var window = GameFacade.Instance.UIManager.LoadWindow("ResultWindow", UIManager.BOARD).GetComponent<ResultWindow>();
                window.Init(Field.Instance.Stage.GetScore());
            }
        }
    }
}

