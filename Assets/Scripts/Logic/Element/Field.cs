using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field m_Instance;
    public static Field Instance{get{return m_Instance;}}

    private FSM<Field> m_FSM;


    private Stage m_Stage;
    public Stage Stage {get{return m_Stage;}}

    private int m_Weight = 5;
    private int m_Height = 6;
    
    private bool m_IsMoved = false;
    public bool IsMoved { 
        get { return m_IsMoved;}
        set { m_IsMoved = value;}
    }

    private Grid[,] m_Grids;
    public Grid[,] Grids {get{ return m_Grids;}}


    private List<CardData> m_CardPool = new List<CardData>();
    private List<Card> m_GhostCards = new List<Card>();
    private List<Card> m_Cards = new List<Card>();
    public List<Card> Cards { get { return m_Cards;}}

    

    void Awake()
    {
        m_Instance = this;

    }

    void OnDestroy()
    {

    }

    void Start()
    {
        m_FSM = new FSM<Field>(this,  new State<Field>[] {
            new State_Idle<Field>(_C.FSMSTATE.IDLE),
            new State_Eliminate<Field>(_C.FSMSTATE.ELIMINATE),
            new State_Chain<Field>(_C.FSMSTATE.CHAIN),
            new State_Check<Field>(_C.FSMSTATE.CHECK),
            new State_Result<Field>(_C.FSMSTATE.RESULT)
        });

        GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();
    }

    public void Enter(int stage)
    {
        m_Stage     = new Stage(GameFacade.Instance.DataCenter.Level.GetStageData(stage));

        m_Weight    = m_Stage.Weight;
        m_Height    = m_Stage.Height;

        EventManager.SendEvent(new GameEvent(EVENT.ONENTERSTAGE, m_Stage));

        InitGrids();
        InitCards();

        m_FSM.Transist(_C.FSMSTATE.IDLE);

        
    }

    public void Dispose()
    {
        m_Stage.Dispose();

        m_Cards.ForEach(c => {
            c.Grid.Card = null;
            c.Entity.Destroy();
        });
        m_Cards.Clear();

        m_GhostCards.ForEach(c => {
            c.Grid.Card = null;
            c.Entity.Destroy();
        });
        m_GhostCards.Clear();
    }

    public void Transist(_C.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }


    void InitGrids()
    {
        m_Grids = new Grid[m_Weight, m_Height];

        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                m_Grids[i, j] = new Grid(i, j, new Vector2((i - ((m_Weight - 1) / 2.0f)) * 145, (j - ((m_Height - 1) / 2.0f)) * 145));
            }
        }

        EventManager.SendEvent(new GameEvent(EVENT.ONINITGRID));
    }

    void InitCards()
    {
        //获取CardData池子
        m_CardPool.Clear();
        foreach (var id in m_Stage.Cards) {
            CardData card_data = GameFacade.Instance.DataCenter.GetCardData(id);
            m_CardPool.Add(card_data);
        }

        //获取空着的Grid
        List<object> grid_list = this.GetEmptyGrids();

        int count = 3;
        List<object> grid_datas = RandomUtility.Pick(count, grid_list);

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid   = grid_datas[i] as Grid;

            int rand    = RandomUtility.Random(0, m_CardPool.Count);
            Card card   = new Card(m_CardPool[rand]);
            card.Grid   = grid;
            grid.Card   = card;
            card.STATE  = _C.CARD_STATE.NORMAL;
            
            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_AddCard(card));

            m_Cards.Add(card);
        }
    }

    public List<Card> AddCards()
    {
        //将残影方块实体化
        m_GhostCards.ForEach(card => {
            if (card.Grid.IsEmpty == true) {
                card.STATE = _C.CARD_STATE.NORMAL;
                card.Grid.Card = card;
                card.Entity.FlushUI();
                m_Cards.Add(card);
            } else {
                Debug.LogError("实体化时，当前坐标已经有方块了：" + card.Grid.X + ", " + card.Grid.Y);
            }
        });
        m_GhostCards.Clear();


        List<Card> add_cards = new List<Card>();

        //获取空着的Grid
        List<object> grid_list = this.GetEmptyGrids();


        int count = RandomUtility.Random(1, 4);
        List<object> grid_datas = RandomUtility.Pick(count, grid_list);

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid = grid_datas[i] as Grid;

            int rand    = RandomUtility.Random(0, m_CardPool.Count);
            Card card   = new Card(m_CardPool[rand]);
            card.STATE  = _C.CARD_STATE.GHOST;
            card.Grid   = grid;

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_AddCard(card));

            add_cards.Add(card);
            m_GhostCards.Add(card);
        }

        return add_cards;
    }


    //获取空位格子
    List<object> GetEmptyGrids()
    {
        //获取空着的Grid
        List<object> grid_list = new List<object>(); 
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                var g = Field.Instance.Grids[i, j];
                if (g.IsEmpty == true && g.IsValid == true) {
                    grid_list.Add(g);
                }  
            }
        }

        return grid_list;
    }

    //清理残影
    //如果当前位置有残影，则清空残影
    void ClearGhost(Card card)
    {
        for (int i = 0; i < m_GhostCards.Count; i++)
        {
            var c = m_GhostCards[i];
            if (c.Grid == card.Grid) {
                m_GhostCards.Remove(c);
                c.Entity.Destroy();
                break;
            }
        }
    }


    bool IsGridSame(Grid g1, Grid g2)
    {
        if (g1 == null || g2 == null) return false;

        if (g1.Card != null && g2.Card != null && g1.Card.ID == g2.Card.ID) {
            return true;
        }
        return false;
    }

    bool IsSameCardNear(Card card)
    {
        Grid card_grid = card.Grid;

        Grid top_grid = null;
        if (card_grid.Y < m_Height - 1) {
            top_grid = m_Grids[card_grid.X, card_grid.Y + 1];
        }

        Grid down_grid = null;
        if (card_grid.Y > 0) {
            down_grid = m_Grids[card_grid.X, card_grid.Y - 1];
        }

        Grid left_grid = null;
        if (card_grid.X > 0) {
            left_grid = m_Grids[card_grid.X - 1, card_grid.Y];
        }

        Grid right_grid = null;
        if (card_grid.X < m_Weight - 1) {
            right_grid = m_Grids[card_grid.X + 1, card_grid.Y];
        }

        if (IsGridSame(card_grid, top_grid)) {
            return true;
        }

        if (IsGridSame(card_grid, down_grid)) {
            return true;
        }

        if (IsGridSame(card_grid, left_grid)) {
            return true;
        }

        if (IsGridSame(card_grid, right_grid)) {
            return true;
        }

        return false;
    }
    
    //获取四个方向相邻的方块
    public Card GetCardByDirection(Card card, _C.DIRECTION direction)
    {
        switch (direction)
        {
            case _C.DIRECTION.TOP:
                if (card.Grid.Y == m_Height - 1)
                    return null;

                var g_top = Field.Instance.Grids[card.Grid.X, card.Grid.Y + 1];
                return g_top.Card;
            

            case _C.DIRECTION.DOWN:
                if (card.Grid.Y == 0)
                    return null;

                var g_down = Field.Instance.Grids[card.Grid.X, card.Grid.Y - 1];
                return g_down.Card;

            
            case _C.DIRECTION.LEFT:
                if (card.Grid.X == 0)
                    return null;

                var g_left = Field.Instance.Grids[card.Grid.X - 1, card.Grid.Y];
                return g_left.Card;


            case _C.DIRECTION.RIGHT:
                if (card.Grid.X == m_Weight - 1)
                    return null;

                var g_right = Field.Instance.Grids[card.Grid.X + 1, card.Grid.Y];
                return g_right.Card;
        }


        return null;
    }

    //获取离得最近的同色方块
    public Card GetMinDistanceSameCard(Card card)
    {
        float min_distance = 1000;
        Vector2 o_pos = card.Grid.Position;

        Card target_card = null;

        m_Cards.ForEach(c => {
            if (c.ID == card.ID && c != card) {
                Vector2 t_pos = c.Grid.Position;
                float dis = Vector2.Distance(o_pos, t_pos);
                if (dis < min_distance) {
                    min_distance = dis;
                    target_card = c;
                }
            }
        });

        return target_card;
    }

    //向上移动(单个)
    public Grid MoveTop(Card card)
    {
        Grid origin = card.Grid;

        if (origin.Y == m_Height - 1) return null;

        Grid target = null;

        for (int j = origin.Y + 1; j < m_Height; j++)
        {
            Grid grid = m_Grids[origin.X, j];
            if (!grid.IsEmpty || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);
        m_Stage.MoveStep.UpdateCurrent(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card));

        return target;
    } 

    //向下移动
    public Grid MoveDown(Card card)
    {
        Grid origin = card.Grid;

        if (origin.Y == 0) return null;

        Grid target = null;

        for (int j = origin.Y -1; j >= 0; j--)
        {
            Grid grid = m_Grids[origin.X, j];
            if (!grid.IsEmpty || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);
        m_Stage.MoveStep.UpdateCurrent(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card));

        return target;
    }

    //向左移动
    public Grid MoveLeft(Card card)
    {
        Grid origin = card.Grid;

        if (origin.X == 0) return null;

        Grid target = null;

        for (int i = origin.X -1; i >= 0; i--)
        {
            Grid grid = m_Grids[i, origin.Y];
            if (!grid.IsEmpty || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);
        m_Stage.MoveStep.UpdateCurrent(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card));

        return target;
    }

    //向右移动
    public Grid MoveRight(Card card)
    {
        Grid origin = card.Grid;

        if (origin.X == m_Weight - 1) return null;

        Grid target = null;

        for (int i = origin.X + 1; i < m_Weight; i++)
        {
            Grid grid = m_Grids[i, origin.Y];
            if (!grid.IsEmpty || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);
        m_Stage.MoveStep.UpdateCurrent(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card));

        return target;
    }

    //计算消除
    //相邻的
    public List<Card> CheckEliminate()
    {
        List<Card> _Removes = new List<Card>();

        m_Cards.ForEach(card => {
            if (IsSameCardNear(card) == true) {
                _Removes.Add(card);
            }
        });


        _Removes.ForEach(c => {
            c.Grid.Card = null;
            // c.Grid = null;       //不置空，否则会影响连锁反应的判断
            m_Cards.Remove(c);
        });

        return _Removes;
    }

    public _C.RESULT CheckResult()
    {
        if (m_Stage.IsFinished() == true) return _C.RESULT.VICTORY;

        if (m_Stage.MoveStep.IsClear() == true) return _C.RESULT.LOSE;


        return _C.RESULT.NONE;
    }

    void Update()
    {
        if (m_FSM != null) m_FSM.Update();
    }




    #region 监听事件

    #endregion
}
