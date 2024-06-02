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
            cards.ForEach(card => {
                EventManager.SendEvent(new GameEvent(EVENT.ONCARDBROKEN, card));
            });
            
            Field.Instance.Transist(_C.FSMSTATE.CHAIN, cards);
        } else {
            if (Field.Instance.IsMoved == true) {
                Field.Instance.Transist(_C.FSMSTATE.RESULT);
            } else {
                Field.Instance.Transist(_C.FSMSTATE.IDLE);
            }
        }
    }

    public override void Update()
    {

    }
}
