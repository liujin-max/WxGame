using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责判断方块的销毁逻辑
public class State_Eliminate<T> : State<Field>
{
    public State_Eliminate(_C.FSMSTATE id) : base(id){}

    private List<Card> m_Removes = new List<Card>();
    private bool m_IsWait = false;

    public override void Enter(params object[] values)
    {
        m_Removes = Field.Instance.CheckEliminate();

        if (m_Removes.Count > 0) {
            m_IsWait = true;
            m_Removes.ForEach(card => {
                EventManager.SendEvent(new GameEvent(EVENT.ONCARDBROKEN, card));
            });
            
            // Field.Instance.Transist(_C.FSMSTATE.CHAIN, m_Removes);
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
        //等动画播完了 再进入下一步
        if (m_IsWait) {
            for (int i = 0; i < m_Removes.Count; i++){
                var card = m_Removes[i];
                if (card.Entity != null) 
                    return;
            }

            Field.Instance.Transist(_C.FSMSTATE.CHAIN, m_Removes);

            m_IsWait = false;
            m_Removes.Clear();
        }
    }
}
