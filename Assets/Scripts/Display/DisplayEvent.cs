using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;




//添加卡牌
public class DisplayEvent_AddCard : DisplayEvent
{
    private CDTimer m_Timer = new CDTimer(0.3f);
    public DisplayEvent_AddCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();
        var card = m_Params[0] as Card;

        // EventManager.SendEvent(new GameEvent(EVENT.ONADDCARD, card));

        card.Display();
    }

    public override void Update(float dealta_time)
    {
        m_Timer.Update(dealta_time);

        if (m_Timer.IsFinished())
            m_State = _C.DISPLAY_STATE.END;
        
    }
}

//移动卡牌
public class DisplayEvent_MoveCard : DisplayEvent
{
    public DisplayEvent_MoveCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        var card = m_Params[0] as Card;
        card.Entity.transform.DOLocalMove(card.Grid.Position, 0.3f).OnComplete(()=>{
            m_State = _C.DISPLAY_STATE.END;

            EventManager.SendEvent(new GameEvent(EVENT.ONCARDMOVED, card));
        });
    }
}

//卡牌消除
public class DisplayEvent_BrokenCard : DisplayEvent
{
    public DisplayEvent_BrokenCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        var card = m_Params[0] as Card;
        card.Entity.transform.DOScale(1.5f, 0.2f);
        card.Entity.DoFade(0f, 0.2f, ()=>{
            m_State = _C.DISPLAY_STATE.END;

            card.Dispose();

            EventManager.SendEvent(new GameEvent(EVENT.UI_BROKENCARD, card));
        });

    }
}



//动画节点
public class DisplayEvent
{
    protected _C.DISPLAY_STATE m_State = _C.DISPLAY_STATE.IDLE;

    protected object[] m_Params;

    public DisplayEvent(params object[] values)
    {
        m_Params = values;
    }

    public virtual void Start()
    {
        m_State = _C.DISPLAY_STATE.PLAYING;
    }

    public virtual void Update(float dealta_time)
    {

    }

    public bool IsIdle()
    {
        return m_State == _C.DISPLAY_STATE.IDLE;
    }

    public bool IsPlaying()
    {
        return m_State == _C.DISPLAY_STATE.PLAYING;
    }

    public bool IsFinished()
    {
        return m_State == _C.DISPLAY_STATE.END;
    }
}
