using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责执行方块的连锁反应
public class State_Chain<T> : State<Field>
{
    public State_Chain(_C.FSMSTATE id) : base(id){}

    private List<Card> m_ChainCards = new List<Card>();

    public override void Enter(params object[] values)
    {
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnCardMoved);

        List<Card> cards = values[0] as List<Card>;

        m_ChainCards.Clear();
        cards.ForEach(c => {
            var top = Field.Instance.GetCardByDirection(c, _C.DIRECTION.TOP);
            if (top != null) {
                if (Field.Instance.MoveTop(top) != null) {
                    m_ChainCards.Add(top);
                }
            }

            var down = Field.Instance.GetCardByDirection(c, _C.DIRECTION.DOWN);
            if (down != null) {
                if (Field.Instance.MoveDown(down) != null) {
                    m_ChainCards.Add(down);
                }
            }

            var left = Field.Instance.GetCardByDirection(c, _C.DIRECTION.LEFT);
            if (left != null) {
                if (Field.Instance.MoveLeft(left) != null) {
                    m_ChainCards.Add(left);
                }
            }

            var right = Field.Instance.GetCardByDirection(c, _C.DIRECTION.RIGHT);
            if (right != null) {
                if (Field.Instance.MoveRight(right) != null) {
                    m_ChainCards.Add(right);
                }
            }
        });
    }

    public override void Update()
    {
        if (m_ChainCards.Count == 0) {
            Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
        }
    }

    public override void Exit()
    {
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }



    private void OnCardMoved(GameEvent @event)
    {
        var card = (@event.GetParam(0) as CardView).Card;

        m_ChainCards.Remove(card);
    }
}
