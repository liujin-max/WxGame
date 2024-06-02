using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Eliminate<T> : State<Field>
{
    public State_Eliminate(_C.FSMSTATE id) : base(id){}



    public override void Enter(params object[] values)
    {
        List<Card> cards = Field.Instance.CheckEliminate();

        if (cards.Count > 0) {
            Debug.Log("State_Eliminate : 进入连锁反应");

            EventManager.SendEvent(new GameEvent(EVENT.UI_DESTROYCARD, this));

            Field.Instance.Transist(_C.FSMSTATE.CHAIN, cards);
        } else {
            if (Field.Instance.IsMoved == true) {
                Debug.Log("State_Eliminate : 进入结算");
                Field.Instance.Transist(_C.FSMSTATE.RESULT);
            } else {
                Debug.Log("State_Eliminate : 进入待机");
                Field.Instance.Transist(_C.FSMSTATE.IDLE);
            }
            
        }
    }

    public override void Update()
    {
        
    }
}
