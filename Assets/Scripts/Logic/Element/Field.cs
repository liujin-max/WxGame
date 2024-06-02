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
    
    private bool m_IsMoved = false;
    public bool IsMoved { 
        get { return m_IsMoved;}
        set { m_IsMoved = value;}
    }

    private Grid[,] m_Grids = new Grid[_C.DEFAULT_WEIGHT, _C.DEFAULT_HEIGHT];
    public Grid[,] Grids {get{ return m_Grids;}}


    private List<Card> m_Cards = new List<Card>();
    public List<Card> Cards { get { return m_Cards;}}

    private List<Card> m_GhostCards = new List<Card>();

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
            new State_Result<Field>(_C.FSMSTATE.RESULT)
        });

    }

    public void Enter()
    {
        InitGrids();

        var window = GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();
        window.Init();

        InitCards();
        m_FSM.Transist(_C.FSMSTATE.IDLE);
    }

    public void Transist(_C.FSMSTATE state, params object[] values)
    {
        m_FSM.Transist(state, values);
    }


    void InitGrids()
    {
        for (int i = 0; i < _C.DEFAULT_WEIGHT; i++) {
            for (int j = 0; j < _C.DEFAULT_HEIGHT; j++) {
                var grid = new Grid(i, j);
                m_Grids[i, j] = grid;
            }
        }
    }

    void InitCards()
    {
        //获取所有CardData
        List<object> card_list = new List<object>();
        card_list.AddRange(GameFacade.Instance.DataCenter.GetCards());

        //获取空着的Grid
        List<object> grid_list = new List<object>(); 
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                var g = Field.Instance.Grids[i, j];
                if (g.IsEmpty() == true) {
                    grid_list.Add(g);
                }  
            }
        }

        int count = 3;
        List<object> grid_datas = RandomUtility.Pick(count, grid_list);

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid = grid_datas[i] as Grid;

            CardData card_data = RandomUtility.Pick(1, card_list)[0] as CardData;

            Card card   = new Card(card_data);
            card.Grid   = grid;
            grid.Card   = card;
            card.STATE  = _C.CARD_STATE.NORMAL;

            EventManager.SendEvent(new GameEvent(EVENT.ONADDCARD, card));

            m_Cards.Add(card);
        }
    }

    public List<Card> AddCards()
    {
        //将残影方块实体化
        m_GhostCards.ForEach(card => {
            if (card.Grid.IsEmpty() == true) {
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

        //获取所有CardData
        List<object> card_list = new List<object>();
        card_list.AddRange(GameFacade.Instance.DataCenter.GetCards());

        //获取空着的Grid
        List<object> grid_list = new List<object>(); 
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                var g = Field.Instance.Grids[i, j];
                if (g.IsEmpty() == true) {
                    grid_list.Add(g);
                }  
            }
        }

        int count = 3;
        List<object> grid_datas = RandomUtility.Pick(count, grid_list);

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid = grid_datas[i] as Grid;

            CardData card_data = RandomUtility.Pick(1, card_list)[0] as CardData;

            Card card   = new Card(card_data);
            card.STATE  = _C.CARD_STATE.GHOST;
            card.Grid   = grid;

            EventManager.SendEvent(new GameEvent(EVENT.ONADDCARD, card));

            add_cards.Add(card);
            m_GhostCards.Add(card);
        }

        return add_cards;
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
        if (card_grid.Y < _C.DEFAULT_HEIGHT - 1) {
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
        if (card_grid.X < _C.DEFAULT_WEIGHT - 1) {
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

    public Card GetCardByDirection(Card card, _C.DIRECTION direction)
    {
        switch (direction)
        {
            case _C.DIRECTION.TOP:
                if (card.Grid.Y == _C.DEFAULT_HEIGHT - 1)
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
                if (card.Grid.X == _C.DEFAULT_WEIGHT - 1)
                    return null;

                var g_right = Field.Instance.Grids[card.Grid.X + 1, card.Grid.Y];
                return g_right.Card;
        }


        return null;
    }

    //向上移动(单个)
    public Grid MoveTop(Card card)
    {
        Grid origin = card.Grid;

        if (origin.Y == _C.DEFAULT_HEIGHT - 1) return null;

        Grid target = null;

        for (int j = origin.Y + 1; j < _C.DEFAULT_HEIGHT; j++)
        {
            Grid grid = m_Grids[origin.X, j];
            if (!grid.IsEmpty() || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

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
            if (!grid.IsEmpty() || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

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
            if (!grid.IsEmpty() || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

        return target;
    }

    //向右移动
    public Grid MoveRight(Card card)
    {
        Grid origin = card.Grid;

        if (origin.X == _C.DEFAULT_WEIGHT - 1) return null;

        Grid target = null;

        for (int i = origin.X + 1; i < _C.DEFAULT_WEIGHT; i++)
        {
            Grid grid = m_Grids[i, origin.Y];
            if (!grid.IsEmpty() || !grid.IsValid) break;
            
            target  = grid;
        }

        if (target == null)  return null;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        this.ClearGhost(card);

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

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

    void Update()
    {
        if (m_FSM != null) m_FSM.Update();
    }


}
