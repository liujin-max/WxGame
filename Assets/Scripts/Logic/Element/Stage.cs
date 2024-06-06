using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//第1关
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

//第2关
public class Matrix_2 : Matrix
{
    public override void InitCards()
    {
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10001), Field.Instance.GetGrid(2, 3));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10001), Field.Instance.GetGrid(2, 1));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10001), Field.Instance.GetGrid(1, 2));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10001), Field.Instance.GetGrid(3, 2));
    }
}

//第3关
public class Matrix_3 : Matrix
{
    public override void FilterGrids()
    {
        Field.Instance.GetGrid(2, 2).IsValid = false;
    }

    public override void InitCards()
    {
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10001), Field.Instance.GetGrid(2, 3));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10002), Field.Instance.GetGrid(2, 1));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10003), Field.Instance.GetGrid(1, 2));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, Field.Instance.Stage.GetCardData(10004), Field.Instance.GetGrid(3, 2));
    }
}

//负责各关卡的特殊处理
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


//关卡数据
public class Stage
{
    private StageData m_Data;

    public int ID {get{return m_Data.ID;}}
    public string Name {get{return "关卡 " + m_Data.ID;}}
    public int Weight {get{return m_Data.Weight;}}
    public int Height {get{return m_Data.Height;}}
    public int Coin {get{return m_Data.Coin;}}


    private List<Condition> m_Conditions = new List<Condition>();
    public List<Condition> Conditions {get{return m_Conditions;}}

    private List<CardData> m_Cards = new List<CardData>();
    private Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    public List<CardData> Cards {get{return this.m_Cards;}}

    //行动次数
    public Pair MoveStep;
    private Matrix m_Matrix;

    private static Dictionary<int, Func<Matrix>> m_classDictionary = new Dictionary<int, Func<Matrix>> {
        { 1, () => new Matrix_1()},
        { 2, () => new Matrix_2()},
        { 3, () => new Matrix_3()},
    };

    public Stage(StageData stageData)
    {
        m_Data  = stageData;

        MoveStep = new Pair(m_Data.Step, m_Data.Step);

        if (m_classDictionary.ContainsKey(this.ID)) {
            m_Matrix = m_classDictionary[this.ID]();
        } else {
            m_Matrix = new Matrix();
        }

        ParseCardPool();
        ParseCondition();
    }

    public void Dispose()
    {
        m_Matrix.Dispose();
    }

    void ParseCardPool()
    {
        m_Cards.Clear();
        m_CardDic.Clear();

        foreach (var id in m_Data.Cards.Split('|')) {
            int card_id = Convert.ToInt32(id);
            CardData card_data = GameFacade.Instance.DataCenter.GetCardData(card_id);
            m_Cards.Add(card_data);
            m_CardDic[card_id] = card_data;
        }
    }

    public CardData GetCardData(int card_id)
    {
        CardData cardData;
        if (m_CardDic.TryGetValue(card_id, out cardData)) {
            return cardData;
        }

        return null;
    }

    void ParseCondition()
    {
        m_Conditions.Clear();

        foreach (var item in m_Data.Condition.Split('|')) {
            string[] conditions = item.Split(':');
            var condition = new Condition(Convert.ToInt32(conditions[0]), Convert.ToInt32(conditions[1]));
            m_Conditions.Add(condition);
        }
    }

    public Condition GetCondition(int condition_id)
    {
        for (int i = 0; i < m_Conditions.Count; i++) {
            var c = m_Conditions[i];
            if (c.ID == condition_id)
                return c;
        }
        return null;
    }

    //判断是否符合
    public bool IsConformTo(int id)
    {
        for (int i = 0; i < m_Conditions.Count; i++) {
            var c = m_Conditions[i];
            if (c.IsConformTo(id) == true)
                return true;
        }

        return false;
    }

    //收集
    public void Collect(int id, int count)
    {
        for (int i = 0; i < m_Conditions.Count; i++) {
            var c = m_Conditions[i];
            if (c.IsConformTo(id) == true) {
                c.Collect(count);
            }
        }
    }

    public bool IsFinished()
    {
        for (int i = 0; i < m_Conditions.Count; i++) {
            if (!m_Conditions[i].IsFinished())
                return false;
        }

        return true;
    }

    public bool IsConditionFinished(int condition_id)
    {
        var c = this.GetCondition(condition_id);
        if (c != null) {
            return c.IsFinished();
        }

        return false;
    }


    #region 逻辑处理
    public void FilterGrids()
    {
        //过滤一遍最终要展示的格子
        m_Matrix.FilterGrids();

        GameFacade.Instance.DisplayEngine.Put(DisplayEngine.Track.Common, new DisplayEvent_ShowAllGrid());
    }

    public void InitCards()
    {
        m_Matrix.InitCards();
    }

    public List<Card> AddCards()
    {
        return m_Matrix.AddCards();
    }    
    #endregion

    #region 监听事件

    #endregion
}
