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
        Debug.Log("State_Idle");
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnCardMoved);
        
        Field.Instance.IsMoved = true;

        //场上没有可移动的方块
        if (Field.Instance.Cards.Count == 0) 
            Field.Instance.Transist(_C.FSMSTATE.CHECK);
    }

    public override void Exit()
    {
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }



    private void OnCardMoved(GameEvent @event)
    {
        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
    }
}
