using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责监听方块的滑动
public class State_Idle<T> : State<Field>
{
    public State_Idle(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnCardMoved);

        _C.RESULT result = Field.Instance.CheckResult();
        if (result != _C.RESULT.NONE)
        {
            Field.Instance.Transist(_C.FSMSTATE.RESULT, result);
            return;
        }
        
        //场上没有可移动的方块
        if (Field.Instance.GetDragableCards().Count == 0) {
            Field.Instance.Transist(_C.FSMSTATE.CHECK);
            return;
        }
            

        //记录当前布局
        Field.Instance.Turn++;
        Field.Instance.RecordHistory();
    }

    public override void Exit()
    {
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }

    public override void Update()
    {
        _C.RESULT result = Field.Instance.CheckResult();
        if (result != _C.RESULT.NONE)
        {
            Field.Instance.Transist(_C.FSMSTATE.RESULT, result);
        }
    }


    private void OnCardMoved(GameEvent @event)
    {
        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
    }
}
