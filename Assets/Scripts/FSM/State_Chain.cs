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

        cards.ForEach(c => {
            if (c.TYPE == _C.CARD_TYPE.JELLY)   //普通方块才有连锁反应
            {
                var top = Field.Instance.GetCardByDirection(c, _C.DIRECTION.UP);
                if (top != null) {
                    top.OnChain(_C.DIRECTION.UP);
                }

                var down = Field.Instance.GetCardByDirection(c, _C.DIRECTION.DOWN);
                if (down != null) {
                    down.OnChain(_C.DIRECTION.DOWN);
                }

                var left = Field.Instance.GetCardByDirection(c, _C.DIRECTION.LEFT);
                if (left != null) {
                    left.OnChain(_C.DIRECTION.LEFT);
                }

                var right = Field.Instance.GetCardByDirection(c, _C.DIRECTION.RIGHT);
                if (right != null) {
                    right.OnChain(_C.DIRECTION.RIGHT);
                }
            }
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
