using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责执行方块消解后的连锁反应
public class State_Chain<T> : State<Field>
{
    public State_Chain(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        List<Card> cards = values[0] as List<Card>;


        foreach (Card c in cards)
        {
            if (c.TYPE == _C.CARD_TYPE.JELLY)   //普通方块才有连锁反应
            {
                foreach (_C.DIRECTION dir in Field.Instance.Directions) {
                    Card card = Field.Instance.GetCardByDirection(c.Grid, dir);
                    if (card != null) {
                        card.OnChain(dir);
                    }
                }
            }
        }

        //在连锁反应后再处理衍生物的问题
        //否则衍生物可能被推走
        cards.ForEach(c => {
            c.OnAfterChain();
        }); 
    }

    public override void Update()
    {
        if (!GameFacade.Instance.DisplayEngine.IsIdle()) return;

        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
    }

    public override void Exit()
    {

    }

}
