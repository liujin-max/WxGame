using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责执行方块的连锁反应
public class State_Chain<T> : State<Field>
{
    public State_Chain(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        List<Card> cards = values[0] as List<Card>;

        cards.ForEach(c => {
            var top = Field.Instance.GetCardByDirection(c, _C.DIRECTION.UP);
            if (top != null) {
                if (Field.Instance.Move(top, _C.DIRECTION.UP) != null) {

                }
            }

            var down = Field.Instance.GetCardByDirection(c, _C.DIRECTION.DOWN);
            if (down != null) {
                if (Field.Instance.Move(down, _C.DIRECTION.DOWN) != null) {

                }
            }

            var left = Field.Instance.GetCardByDirection(c, _C.DIRECTION.LEFT);
            if (left != null) {
                if (Field.Instance.Move(left, _C.DIRECTION.LEFT) != null) {

                }
            }

            var right = Field.Instance.GetCardByDirection(c, _C.DIRECTION.RIGHT);
            if (right != null) {
                if (Field.Instance.Move(right, _C.DIRECTION.RIGHT) != null) {

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
