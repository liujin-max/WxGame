using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Result<T> : State<Field>
{
    private CDTimer m_Timer = new CDTimer(0.3f);

    public State_Result(_C.FSMSTATE id) : base(id){}

    public override void Enter(params object[] values)
    {
        m_Timer.Reset();

        Field.Instance.IsMoved = false;

        Field.Instance.AddCards();
        
    }

    public override void Update()
    {
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished() == true) {
            m_Timer.Reset();

            Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
        }
    }
}

