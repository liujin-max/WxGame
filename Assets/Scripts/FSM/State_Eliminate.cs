using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责判断方块的销毁逻辑
public class State_Eliminate<T> : State<Field>
{
    public State_Eliminate(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        List<Card> _removes = Field.Instance.CheckEliminate();

        //有宝石要消除，则消除并进入连锁反应
        if (_removes.Count > 0) {
            _removes.ForEach(card => {
                Field.Instance.Stage.Collect(card.ID, 1);

                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_BrokenCard(card));
            });
            
            Field.Instance.Transist(_C.FSMSTATE.CHAIN, _removes);
        } else {
            //无宝石需要消除
            //先判断是否满足胜利或失败条件
            //然后判断是否行动过，未行动则进入行动阶段，否则进入下一回合
            _C.RESULT result = Field.Instance.CheckResult();
            if (result == _C.RESULT.NONE)
            {
                if (Field.Instance.IsMoved == true) {
                    Field.Instance.Transist(_C.FSMSTATE.CHECK);
                } else {
                    Field.Instance.Transist(_C.FSMSTATE.IDLE);
                }
            }
            else
            {
                Field.Instance.Transist(_C.FSMSTATE.RESULT, result);
            }
        }
    }
}
