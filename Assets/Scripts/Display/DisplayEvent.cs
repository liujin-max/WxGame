using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;



//空白 单纯的等待时间
public class DisplayEvent_Wait : DisplayEvent
{
    private CDTimer m_Timer = new CDTimer(1);
    public DisplayEvent_Wait(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        m_Timer = new CDTimer((float)m_Params[0]);
    }

    public override void Update(float dt)
    {
        m_Timer.Update(dt);
        if (m_Timer.IsFinished()) {
           m_State = _C.DISPLAY_STATE.END; 
        }
    }

}

//展现所有格子
public class DisplayEvent_ShowAllGrid : DisplayEvent
{
    public DisplayEvent_ShowAllGrid(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        //展示所有格子
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++)
        {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++)
            {
                var grid = Field.Instance.Grids[i, j];

                if (grid.IsValid) {
                   if (grid.Entity == null) grid.Display();

                    grid.Entity.transform.localScale = Vector3.one; 
                    grid.Show(true);
                } else {
                    grid.Show(false);
                }
            }
        }
        

        m_State = _C.DISPLAY_STATE.END;
    }

}

//隐藏格子
public class DisplayEvent_HideGrid : DisplayEvent
{
    private CDTimer m_Timer = new CDTimer(0.1f);
    public DisplayEvent_HideGrid(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        var grid = m_Params[0] as Grid;

        if (grid.Entity == null) {
            m_State = _C.DISPLAY_STATE.END;
            return;
        }
        

        grid.Frame.transform.DOScale(0f, 0.2f);
    }

    public override void Update(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (m_Timer.IsFinished()) {
            m_State = _C.DISPLAY_STATE.END;
        }
    }
}



//添加虚化方块
public class DisplayEvent_GhostCard : DisplayEvent
{
    public DisplayEvent_GhostCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();
        var card = m_Params[0] as Card;

        card.Display();

        m_State = _C.DISPLAY_STATE.END;
    }
}

//添加实体方块
public class DisplayEvent_NormalCard : DisplayEvent
{
    public DisplayEvent_NormalCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();
        var card = m_Params[0] as Card;

        if (card.Entity == null) {
            card.Display();
        } else {
            card.Entity.Flush();
        }

        if (card.TYPE == _C.CARD_TYPE.FRAME) {
            card.Entity.SetPosition(card.Grid.Position);
            m_State = _C.DISPLAY_STATE.END;

        } else {
            card.Entity.Entity.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            card.Entity.DoScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

            card.Entity.transform.DOJump(card.Grid.Position, 0.5f, 1, 0.25f).SetEase(Ease.OutQuad).OnComplete(() => {
                m_State = _C.DISPLAY_STATE.END;

                card.Entity.Shake(new Vector2(0.03f, 0.03f));
            });
        }
    }
}

//移动卡牌
public class DisplayEvent_MoveCard : DisplayEvent
{
    private List<Grid> m_GridPaths = new List<Grid>();
    private CDTimer m_Timer;

    public DisplayEvent_MoveCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        var card        = m_Params[0] as Card;
        var direction   = (_C.DIRECTION)m_Params[1];
        m_GridPaths     = (List<Grid>)m_Params[2];

        m_Timer = new CDTimer(0.05f);
        m_Timer.Full();

        if (direction == _C.DIRECTION.LEFT || direction == _C.DIRECTION.RIGHT)
        {
            card.Entity.DoScale(new Vector3(1.2f, 0.8f, 0), 0.1f);
        }
        else
        {
            card.Entity.DoScale(new Vector3(0.8f, 1.2f, 0), 0.1f);
        }
    }

    public override void Update(float dt)
    {
        m_Timer.Update(dt);
        if (m_Timer.IsFinished() == true) {
            m_Timer.ForceReset();

            var card        = m_Params[0] as Card;
            var direction   = (_C.DIRECTION)m_Params[1];

            if (m_GridPaths.Count > 0)
            {
                var grid = m_GridPaths.First();
                m_GridPaths.Remove(grid);
                
                Vector2 to_pos = grid.Position;
                if (Vector3.Distance(card.Entity.transform.localPosition, to_pos) >= 1.5f)
                {
                    m_Timer.Full();

                    card.Entity.transform.localPosition = to_pos;
                }
                else
                {
                    card.Entity.transform.DOLocalMove(to_pos, m_Timer.Duration).SetEase(Ease.Linear);
                }
                
            }
            else
            {
                m_State = _C.DISPLAY_STATE.END;

                card.Entity.transform.localPosition = card.Grid.Position;
                card.Entity.DoScale(Vector3.one, 0.1f); //不在缩放结束后处理了，有可能被顶掉

                var hit = Field.Instance.GetCardByDirection(card, direction);
                if (hit != null) {
                    hit.Entity.Shake(direction);
                }
            }
            
        }
    }

    public override void Terminate()
    {
        var card        = m_Params[0] as Card;

        EventManager.SendEvent(new GameEvent(EVENT.ONCARDMOVED, card));
        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATESTEP, false));
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

        if (card.IsEliminating) {
            m_State = _C.DISPLAY_STATE.END;
            return;
        }

        card.IsEliminating = true;

        //收集
        Field.Instance.Stage.Collect(card.ID, 1);
    }

    public override void Update(float dealta_time)
    {
        var card = m_Params[0] as Card;

        float height = card.Entity.Entity.size.y + dealta_time * 3;
        card.Entity.Entity.size = new Vector2(card.Entity.Entity.size.x, height);

        RectTransform rect = card.Entity.Entity.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);

        if (card.Entity.Entity.size.y >= 1.9f) {
            m_State = _C.DISPLAY_STATE.END;

            var broken = GameFacade.Instance.EffectManager.Load(EFFECT.BROKEN, card.Entity.transform.position).GetComponent<BrokenEffect>();
            broken.Init(card.ID);

            card.Dispose();

            EventManager.SendEvent(new GameEvent(EVENT.ONBROKENCARD, card));

            EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECONDITION));
        }
    }
}

//打乱卡牌
public class DisplayEvent_ShuffleCard : DisplayEvent
{
    private CDTimer m_Timer = new CDTimer(0.3f);
    public DisplayEvent_ShuffleCard(params object[] values) : base(values) {}

    public override void Start()
    {
        base.Start();

        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, true));

        var cards = m_Params[0] as List<Card>;

        cards.ForEach(card => {
            card.Entity.transform.DOLocalMove(card.Grid.Position, 0.2f).OnComplete(()=>{
                card.Entity.DoScale(Vector3.one, 0.1f); //不在缩放结束后处理了，有可能被顶掉
            });
        });
    }

    public override void Update(float deltaTime)
    {
        m_Timer.Update(deltaTime);
        if (!m_Timer.IsFinished()) return;

     
        m_State = _C.DISPLAY_STATE.END;
    }

    public override void Terminate()
    {
        EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPMASK, false));

        Field.Instance.Transist(_C.FSMSTATE.ELIMINATE);
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

    public virtual void Terminate()
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
