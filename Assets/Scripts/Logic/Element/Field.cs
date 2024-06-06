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

    private Land m_Land;
    public Land Land {get{return m_Land;}}

    private int m_Weight = 5;
    private int m_Height = 6;
    
    public bool IsMoved = false;

    private Grid[,] m_Grids;
    public Grid[,] Grids {get{ return m_Grids;}}


    private List<Card> m_Cards = new List<Card>();
    public List<Card> Cards { get { return m_Cards;}}

    private List<Card> m_GhostCards = new List<Card>();
    public List<Card> GhostCards { get { return m_GhostCards;}}


    

    void Awake()
    {
        m_Instance = this;

        m_Land = new Land();
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

        m_Stage.FilterGrids();
        m_Stage.InitCards();


        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    public void Dispose()
    {
        IsMoved = false;

        m_Stage.Dispose();

        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                var grid = m_Grids[i, j];
                grid.Dispose();
            }
        }

        m_Cards.ForEach(c => {
            c.Dispose();
        });
        m_Cards.Clear();

        m_GhostCards.ForEach(c => {
            c.Dispose();
        });
        m_GhostCards.Clear();

        
    }

    public void Transist(_C.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }

    public _C.FSMSTATE GetCurrentFSMState()
    {
        return m_FSM.CurrentState.ID;
    }


    void InitGrids()
    {
        m_Grids = new Grid[m_Weight, m_Height];

        for (int i = 0; i < m_Weight; i++) {
            for (int j = 0; j < m_Height; j++) {
                var grid = new Grid(i, j, new Vector2((i - ((m_Weight - 1) / 2.0f)) * 1.22f, (j - ((m_Height - 1) / 2.0f)) * 1.22f));
                m_Grids[i, j] = grid;
            }
        }
    }

    //添加果冻
    public void PutCard(_C.CARD_STATE state, Card card, Grid grid)
    {
        card.STATE  = state;
        if (state == _C.CARD_STATE.GHOST) {
            card.Grid   = grid;

            m_GhostCards.Add(card);

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_GhostCard(card));

        } else {
            card.Grid   = grid;
            grid.Card   = card;

            m_Cards.Add(card);

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_NormalCard(card));
        }
    }

    public Card PutCard(_C.CARD_STATE state, CardData cardData, Grid grid)
    {
        Card card   = new Card(cardData);
        PutCard(state, card, grid);

        return card;
    }

    public List<Card> AddCards()
    {
        return m_Stage.AddCards();
    }


    //获取空位格子
    public List<object> GetEmptyGrids()
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

    //获取格子
    public Grid GetGrid(int x, int y)
    {
        return m_Grids[x, y];
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
                c.Dispose();
                break;
            }
        }
    }

    void UpdateMoveStep(int value)
    {
        m_Stage.MoveStep.UpdateCurrent(value);
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

        int offset  = Mathf.Abs(target.Y - origin.Y);

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        ClearGhost(card);
        UpdateMoveStep(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.TOP, offset));

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

        int offset  = Mathf.Abs(target.Y - origin.Y);

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        ClearGhost(card);
        UpdateMoveStep(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.DOWN, offset));

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

        int offset  = Mathf.Abs(target.X - origin.X);

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        ClearGhost(card);
        UpdateMoveStep(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.LEFT, offset));

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

        int offset  = Mathf.Abs(target.X - origin.X);

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        ClearGhost(card);
        UpdateMoveStep(-1);

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_MoveCard(card, _C.DIRECTION.RIGHT, offset));

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
