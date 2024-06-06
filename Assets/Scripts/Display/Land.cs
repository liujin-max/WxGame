using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//第一关
public class Matrix_1 : Matrix
{
    private int m_Step = 0;
    public Matrix_1()
    {
        EventManager.AddHandler(EVENT.ONBROKENCARD,     OnBrokenCard);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBROKENCARD,     OnBrokenCard);
    }
    
    public override void FilterGrids()
    {
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++)
        {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++)
            {
                if (j != 2) {
                    var grid = Field.Instance.Grids[i, j];
                    grid.IsValid = false;
                } else {
                    if (i < 1 || i > 5) {
                        var grid = Field.Instance.Grids[i, j];
                        grid.IsValid = false;
                    }
                }
            }
        }
    }

    public override void InitCards()
    {
        CardData cardData = Field.Instance.Stage.GetCardData(10001);

        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(1, 2));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(5, 2)).IsFixed = true;
    }

    public override List<Card> AddCards()
    {
        List<Card> add_cards = new List<Card>();

        //黄
        CardData yellow = Field.Instance.Stage.GetCardData(10002);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(3, 0));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(2, 4)).IsFixed = true;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(4, 4)).IsFixed = true;

        //蓝
        CardData blue   = Field.Instance.Stage.GetCardData(10003);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(0, 3)).IsFixed = true;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(1, 4)).IsFixed = true;
        
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(6, 3)).IsFixed = true;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(5, 4)).IsFixed = true;


        //绿
        CardData green  = Field.Instance.Stage.GetCardData(10004);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(0, 0)).IsFixed = true;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(0, 2)).IsFixed = true;

        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(6, 0)).IsFixed = true;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(6, 2)).IsFixed = true;



        return add_cards; 
    }

    private void OnBrokenCard(GameEvent @event)
    {
        var card = @event.GetParam(0) as Card;

        if (m_Step == 0 && card.ID == 10001 && Field.Instance.Stage.IsConditionFinished(10001) == true) {
            m_Step = 1;

            //隐藏格子
            for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
                var grid = Field.Instance.Grids[i, 2];
                GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Queue, new DisplayEvent_HideGrid(grid));
            }

            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Queue, new DisplayEvent_Wait(0.2f));

            //展示新的格子
            for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++)
            {
                for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++)
                {
                    var grid = Field.Instance.Grids[i, j];
                    grid.IsValid = true;
                }
            }

            Field.Instance.GetGrid(1, 0).IsValid = false;
            Field.Instance.GetGrid(1, 1).IsValid = false;
            Field.Instance.GetGrid(1, 2).IsValid = false;
            Field.Instance.GetGrid(1, 3).IsValid = false;
            Field.Instance.GetGrid(2, 0).IsValid = false;
            Field.Instance.GetGrid(2, 1).IsValid = false;
            Field.Instance.GetGrid(2, 2).IsValid = false;
            Field.Instance.GetGrid(2, 3).IsValid = false;

            Field.Instance.GetGrid(4, 0).IsValid = false;
            Field.Instance.GetGrid(4, 1).IsValid = false;
            Field.Instance.GetGrid(4, 2).IsValid = false;
            Field.Instance.GetGrid(4, 3).IsValid = false;
            Field.Instance.GetGrid(5, 0).IsValid = false;
            Field.Instance.GetGrid(5, 1).IsValid = false;
            Field.Instance.GetGrid(5, 2).IsValid = false;
            Field.Instance.GetGrid(5, 3).IsValid = false;


            GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Queue, new DisplayEvent_ShowAllGrid());
        }
    }



    
}


public class Matrix
{
    public virtual void FilterGrids()
    {

    }


    //初始生成：默认随机生成
    public virtual void InitCards()
    {
        Stage stage = Field.Instance.Stage;

        //获取空着的Grid
        List<object> grid_list = Field.Instance.GetEmptyGrids();

        int count = 3;
        List<object> grid_datas = RandomUtility.Pick(count, grid_list);

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid   = grid_datas[i] as Grid;
            int rand    = RandomUtility.Random(0, stage.Cards.Count);

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, stage.Cards[rand], grid);
        }
    }

    //默认
    public virtual List<Card> AddCards()
    {
        //将残影方块实体化
        Field.Instance.GhostCards.ForEach(card => {
            if (card.Grid.IsEmpty == true) {
                Field.Instance.PutCard(_C.CARD_STATE.NORMAL, card, card.Grid);
            } else {
                Debug.LogError("实体化时，当前坐标已经有方块了：" + card.Grid.X + ", " + card.Grid.Y);
            }
        });
        Field.Instance.GhostCards.Clear();


        List<Card> add_cards = new List<Card>();

        //获取空着的Grid
        int count = RandomUtility.Random(1, 4);
        List<object> grid_datas = RandomUtility.Pick(count, Field.Instance.GetEmptyGrids());

        for (int i = 0; i < grid_datas.Count; i++)
        {
            Grid grid   = grid_datas[i] as Grid;

            int rand    = RandomUtility.Random(0, Field.Instance.Stage.Cards.Count);
            Card card   = Field.Instance.PutCard(_C.CARD_STATE.GHOST, Field.Instance.Stage.Cards[rand], grid);

            add_cards.Add(card);
        }

        return add_cards;
    }

    public virtual void Dispose()
    {

    }
}


//负责对每个关卡做特殊处理
public class Land
{
    public Transform GRID_ROOT;
    public Transform ENTITY_ROOT;

    private Stage m_Stage;
    private Matrix m_Matrix;

    private static Dictionary<int, Func<Matrix>> m_classDictionary = new Dictionary<int, Func<Matrix>> {
        { 1, () => new Matrix_1()},
    };


    public Land()
    {
        GRID_ROOT   = GameObject.Find("Field/Grids").transform;
        ENTITY_ROOT = GameObject.Find("Field/Entitys").transform;
    }

    public void Enter(Stage stage)
    {
        m_Stage = stage;


        if (m_classDictionary.ContainsKey(stage.ID)) {
            m_Matrix = m_classDictionary[stage.ID]();
        } else {
            m_Matrix = new Matrix();
        }
    }

    //
    public void InitGrids()
    {
        //过滤一遍最终要展示的格子
        m_Matrix.FilterGrids();

        DisplayGrids();
    }

    void DisplayGrids()
    {
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++)
        {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++)
            {
                var grid = Field.Instance.Grids[i, j];
                if (grid.IsValid == true)
                    grid.Display();
            }
        }
    }

    public void InitCards()
    {
        m_Matrix.InitCards();
    }

    public List<Card> AddCards()
    {
        return m_Matrix.AddCards();
    }


    public void Dispose()
    {
        m_Matrix.Dispose();
    }
}
