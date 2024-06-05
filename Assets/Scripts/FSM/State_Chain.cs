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
            var top = Field.Instance.GetCardByDirection(c, _C.DIRECTION.TOP);
            if (top != null) {
                top.Entity.Shake(new Vector2(0, 0.2f));

                if (Field.Instance.MoveTop(top) != null) {

                }
            }

            var down = Field.Instance.GetCardByDirection(c, _C.DIRECTION.DOWN);
            if (down != null) {
                down.Entity.Shake(new Vector2(0, 0.2f));

                if (Field.Instance.MoveDown(down) != null) {

                }
            }

            var left = Field.Instance.GetCardByDirection(c, _C.DIRECTION.LEFT);
            if (left != null) {
                left.Entity.Shake(new Vector2(0.2f, 0));

                if (Field.Instance.MoveLeft(left) != null) {

                }
            }

            var right = Field.Instance.GetCardByDirection(c, _C.DIRECTION.RIGHT);
            if (right != null) {
                right.Entity.Shake(new Vector2(0.2f, 0));

                if (Field.Instance.MoveRight(right) != null) {

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
