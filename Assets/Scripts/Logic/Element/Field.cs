using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    private static Field m_Instance;
    public static Field Instance{get{return m_Instance;}}

    private Grid[,] m_Grids = new Grid[_C.DEFAULT_WEIGHT, _C.DEFAULT_HEIGHT];
    public Grid[,] Grids {get{ return m_Grids;}}


    private Card[,] m_CardDic = new Card[_C.DEFAULT_WEIGHT, _C.DEFAULT_HEIGHT];
    private List<Card> m_Cards = new List<Card>();
    public List<Card> Cards { get { return m_Cards;}}

    void Awake()
    {
        m_Instance = this;


        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }

    public void Enter()
    {
        InitGrids();

        var window = GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();
        window.Init();


        AddCards();
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

    public void AddCards()
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

            Card card = new Card(card_data);
            card.Grid = grid;
            grid.Card = card;

            EventManager.SendEvent(new GameEvent(EVENT.ONADDCARD, card));

            m_Cards.Add(card);
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


    //向上移动(单个)
    public void MoveTop(Card card)
    {
        Grid origin = card.Grid;

        if (origin.Y == _C.DEFAULT_HEIGHT - 1) return;

        Grid target = null;

        for (int j = origin.Y + 1; j < _C.DEFAULT_HEIGHT; j++)
        {
            Grid grid = m_Grids[origin.X, j];
            if (grid.Card != null) break;
            
            target  = grid;
        }

        if (target == null)  return;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

        Eliminate();
    } 

    //向下移动
    public void MoveDown(Card card)
    {
        Grid origin = card.Grid;

        if (origin.Y == 0) return;

        Grid target = null;

        for (int j = origin.Y -1; j >= 0; j--)
        {
            Grid grid = m_Grids[origin.X, j];
            if (grid.Card != null) break;
            
            target  = grid;
        }

        if (target == null)  return;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

        Eliminate();
    }

    //向左移动
    public void MoveLeft(Card card)
    {
        Grid origin = card.Grid;

        if (origin.X == 0) return;

        Grid target = null;

        for (int i = origin.X -1; i >= 0; i--)
        {
            Grid grid = m_Grids[i, origin.Y];
            if (grid.Card != null) break;
            
            target  = grid;
        }

        if (target == null)  return;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));

        Eliminate();
    }

    //向右移动
    public void MoveRight(Card card)
    {
        Grid origin = card.Grid;

        if (origin.X == _C.DEFAULT_WEIGHT - 1) return;

        Grid target = null;

        for (int i = origin.X + 1; i < _C.DEFAULT_WEIGHT; i++)
        {
            Grid grid = m_Grids[i, origin.Y];
            if (grid.Card != null) break;
            
            target  = grid;
        }

        if (target == null)  return;

        origin.Card = null;
        target.Card = card;
        card.Grid   = target;

        EventManager.SendEvent(new GameEvent(EVENT.UI_MOVECARD, card));


        Eliminate();
    }

    //计算消除
    //相邻的
    public void Eliminate()
    {
        List<Card> _Removes = new List<Card>();

        m_Cards.ForEach(card => {
            if (IsSameCardNear(card) == true) {
                _Removes.Add(card);
            }
        });


        _Removes.ForEach(c => {
            c.IsEliminate = true;
            c.Grid.Card = null;
            c.Grid = null;
            m_Cards.Remove(c);
        });
    }



    #region 监听事件
    private void OnCardMoved(GameEvent @event)
    {
        //销毁
        EventManager.SendEvent(new GameEvent(EVENT.UI_DESTROYCARD, this));



        //移动结束后的额外处理
        this.AddCards();
        this.Eliminate();
        EventManager.SendEvent(new GameEvent(EVENT.UI_DESTROYCARD, this));
    }
    #endregion
}
