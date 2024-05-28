using System.Collections;
using System.Collections.Generic;
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
    }

    public void Enter()
    {
        InitGrids();
        InitCards();

        var window = GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();
        window.Init();
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
        m_Cards.Clear();


        //获取所有CardData
        List<object> card_list = new List<object>();
        card_list.AddRange(GameFacade.Instance.DataCenter.GetCards());

        //获取所有Grid
        List<object> grid_list = new List<object>(); 
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                grid_list.Add(Field.Instance.Grids[i, j]);
            }
        }

        List<object> card_datas = RandomUtility.Pick(20, card_list);
        List<object> grid_datas = RandomUtility.Pick(20 * 4, grid_list);

        for (int i = 0; i < card_datas.Count; i++)
        {
            CardData card_data = card_datas[i] as CardData;

            for (int j = 0; j < 4; j++)
            {
                Card card = new Card(card_data);

                List<object> grids = RandomUtility.Pick(1, grid_datas);
                Grid grid = grids[0] as Grid;
                card.Grid = grid;
                grid.Card = card;

                m_Cards.Add(card);
                grid_datas.Remove(grid);
            }
        }
    }

    //向上移动
    public void MoveTop()
    {
        for (int i = 0; i < _C.DEFAULT_WEIGHT; i++)
        {
            //从高到低 先筛选出这一列所有的Card
            List<Card> cards = new List<Card>();
            for (int j = _C.DEFAULT_HEIGHT - 1; j >= 0; j--)
            {
                Grid grid = m_Grids[i, j];
                if (grid.Card != null) {
                    cards.Add(grid.Card);
                    grid.Card = null;
                }
            }
            
            for (int j = 0; j < cards.Count; j++) {
                int height = _C.DEFAULT_HEIGHT - j - 1;
                Grid g = m_Grids[i, height];
                Card c = cards[j];
                c.Grid  = g;
                g.Card  = c;
            }
        }

        Eliminate();
    }

    //向下移动
    public void MoveDown()
    {
        for (int i = 0; i < _C.DEFAULT_WEIGHT; i++)
        {
            //从低到高 先筛选出这一列所有的Card
            List<Card> cards = new List<Card>();
            for (int j = 0; j < _C.DEFAULT_HEIGHT; j++)
            {
                Grid grid = m_Grids[i, j];
                if (grid.Card != null) {
                    cards.Add(grid.Card);
                    grid.Card = null;
                }
            }
            
            for (int j = 0; j < cards.Count; j++) {
                int height = j;
                Grid g = m_Grids[i, height];
                Card c = cards[j];
                c.Grid  = g;
                g.Card  = c;
            }
        }

        Eliminate();
    }

    //向左移动
    public void MoveLeft()
    {
        for (int j = 0; j < _C.DEFAULT_HEIGHT; j++)
        {
            // 从左到右筛选
            List<Card> cards = new List<Card>();
            for (int i = 0; i < _C.DEFAULT_WEIGHT; i++)
            {
                Grid grid = m_Grids[i, j];
                if (grid.Card != null) {
                    cards.Add(grid.Card);
                    grid.Card = null;
                }
            }

            for (int i = 0; i < cards.Count; i++) {
                int weight = i;
                Grid g = m_Grids[weight, j];
                Card c = cards[i];
                c.Grid  = g;
                g.Card  = c;
            }
        }

        Eliminate();
    }

    //向右移动
    public void MoveRight()
    {
        for (int j = 0; j < _C.DEFAULT_HEIGHT; j++)
        {
            // 从左到右筛选
            List<Card> cards = new List<Card>();
            for (int i = _C.DEFAULT_WEIGHT - 1; i >= 0; i--) {
                Grid grid = m_Grids[i, j];
                if (grid.Card != null) {
                    cards.Add(grid.Card);
                    grid.Card = null;
                }
            }

            for (int i = 0; i < cards.Count; i++) {
                int weight = _C.DEFAULT_WEIGHT - i - 1;
                Grid g = m_Grids[weight, j];
                Card c = cards[i];
                c.Grid  = g;
                g.Card  = c;
            }
        }

        Eliminate();
    }

    //计算消除
    public void Eliminate()
    {
        List<Card> _Removes = new List<Card>();

        for (int i = m_Cards.Count - 1; i >= 0; i--)
        {
            var card = m_Cards[i];
            if (IsSameCardNear(card) == true) {
                _Removes.Add(card);
            }
        }

        _Removes.ForEach(c => {
            c.IsEliminate = true;
            c.Grid.Card = null;
            m_Cards.Remove(c);
        });
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
}
