using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 第1关
public class Matrix_1 : Matrix
{
    private int m_Step = 0;
    public Matrix_1()
    {
        EventManager.AddHandler(EVENT.ONBROKENCARD,     OnBrokenCard);
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnCardMoved);
    }

    public override void Dispose()
    {
        EventManager.DelHandler(EVENT.ONBROKENCARD,     OnBrokenCard);
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnCardMoved);
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

    public override void InitCards(int count)
    {
        CardData cardData = GameFacade.Instance.DataCenter.GetCardData(10001);

        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(1, 2));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(5, 2)).Dragable = false;

        EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWGUIDE, 1, true));
    }

    public override List<Card> AddCards()
    {
        List<Card> add_cards = new List<Card>();

        //黄
        CardData yellow = GameFacade.Instance.DataCenter.GetCardData(10002);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(3, 0));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(2, 4)).Dragable = false;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, yellow, Field.Instance.GetGrid(4, 4)).Dragable = false;

        //蓝
        CardData blue   = GameFacade.Instance.DataCenter.GetCardData(10003);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(0, 3)).Dragable = false;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(1, 4)).Dragable = false;
        
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(6, 3)).Dragable = false;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, blue, Field.Instance.GetGrid(5, 4)).Dragable = false;


        //绿
        CardData green  = GameFacade.Instance.DataCenter.GetCardData(10004);
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(0, 0)).Dragable = false;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(0, 2)).Dragable = false;

        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(6, 0)).Dragable = false;
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, green, Field.Instance.GetGrid(6, 2)).Dragable = false;

        EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWGUIDE, 2, true));

        return add_cards; 
    }

    private void OnCardMoved(GameEvent @event)
    {
        var card = @event.GetParam(0) as Card;

        if (m_Step == 0 && card.ID == 10001) 
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWGUIDE, 1, false));
        }

        if (m_Step == 1 && card.ID != 10001) 
        {
            EventManager.SendEvent(new GameEvent(EVENT.UI_SHOWGUIDE, 2, false));
        }
    }


    private void OnBrokenCard(GameEvent @event)
    {
        var card = @event.GetParam(0) as Card;

        if (m_Step == 0 && card.ID == 10001) {
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
#endregion


#region 第10关
public class Matrix_10 : Matrix
{
    public override void InitCards(int count)
    {
        List<CardData> card_datas = new List<CardData>();

        foreach (var cardData in  m_Stage.Cards)
        {
            for (int j = 0; j < 8; j++)
            {
                card_datas.Add(cardData);
            }
        }

        List<object> grid_datas = Field.Instance.GetEmptyGrids();

        for (int i = card_datas.Count - 1; i >= 0; i--)
        {
            var data    = card_datas[i];

            int rand    = RandomUtility.Random(0, grid_datas.Count);
            Grid grid   = grid_datas[rand] as Grid;
            
            while (Field.Instance.GetSameCardNear(grid, data.ID).Count > 0)
            {
                rand    = RandomUtility.Random(0, grid_datas.Count);
                grid    = grid_datas[rand] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }
    }

    public override List<Card> AddCards()
    {
        return new List<Card>();
    }
}
#endregion


#region 第11关
public class Matrix_11 : Matrix
{
    public override void InitCards(int count)
    {
        m_Stage.GridJSONs.ForEach(grid_json => {
            if (grid_json.JellyID > 0) {
                var grid    = Field.Instance.GetGrid(grid_json.X, grid_json.Y);
                Field.Instance.PutCard(_C.CARD_STATE.NORMAL, GameFacade.Instance.DataCenter.GetCardData(grid_json.JellyID), grid);
            }
        });



        List<CardData> card_datas = new List<CardData>();

        foreach (var cardData in  m_Stage.Cards)
        {
            for (int j = 0; j < 8; j++)
            {
                card_datas.Add(cardData);
            }
        }

        List<object> grid_datas = Field.Instance.GetEmptyGrids();

        for (int i = card_datas.Count - 1; i >= 0; i--)
        {
            var data    = card_datas[i];

            int rand    = RandomUtility.Random(0, grid_datas.Count);
            Grid grid   = grid_datas[rand] as Grid;
            
            while (Field.Instance.GetSameCardNear(grid, data.ID).Count > 0)
            {
                rand    = RandomUtility.Random(0, grid_datas.Count);
                grid    = grid_datas[rand] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }
    }

    public override List<Card> AddCards()
    {
        return new List<Card>();
    }
}
#endregion


#region 第12关
public class Matrix_12 : Matrix
{
    public override List<Card> AddCards()
    {
        //将虚化方块实体化
        Field.Instance.CorporealCards();

        List<Card> cards    = new List<Card>();

        //上下两个区域，各生成2个虚化方块
        //上区域只生成10001和10004
        //下区域只生成10002和10003

        List<object> top_grids  = new List<object>();
        int[] top_cards         = {10001, 10004};

        List<object> bom_grids  = new List<object>();
        int[] bottom_cards      = {10002, 10003};

        Field.Instance.GetEmptyGrids().ForEach(o => {
            Grid g = o as Grid;

            if (g.Y >= 4) {
                top_grids.Add(o);
            } else {
                bom_grids.Add(o);
            }   
        });

        //
        List<object> tops   = RandomUtility.Pick(2, top_grids);
        List<object> bottoms= RandomUtility.Pick(2, bom_grids);
        
        foreach (Grid g in tops)
        {
            int card_id = top_cards[RandomUtility.Random(0, top_cards.Length)];
            var c = Field.Instance.PutCard(_C.CARD_STATE.GHOST, GameFacade.Instance.DataCenter.GetCardData(card_id), g);

            cards.Add(c);
        }

        foreach (Grid g in bottoms)
        {
            int card_id = bottom_cards[RandomUtility.Random(0, bottom_cards.Length)];
            var c = Field.Instance.PutCard(_C.CARD_STATE.GHOST, GameFacade.Instance.DataCenter.GetCardData(card_id), g);

            cards.Add(c);
        }

        return cards;
    }
}
#endregion


#region 第17关
public class Matrix_17 : Matrix
{
    public override List<Card> AddCards()
    {
        //将虚化方块实体化
        Field.Instance.CorporealCards();

        List<Card> cards    = new List<Card>();

        //只在下半区域生成
        List<Grid> grids  = new List<Grid>();

        Field.Instance.GetEmptyGrids().ForEach(o => {
            Grid g = o as Grid;

            if (g.Y <= 2) {
                grids.Add(g);
            }  
        });

        int count = Mathf.Min(RandomUtility.Random(1, 3), grids.Count);
        for (int i = 0; i < count; i++)
        {
            var data    = m_Stage.Cards[RandomUtility.Random(0, m_Stage.Cards.Count)];
            Grid grid   = grids[RandomUtility.Random(0, grids.Count)];
            Card card   = Field.Instance.PutCard(_C.CARD_STATE.GHOST, data, grid);

            grids.Remove(grid);
            cards.Add(card);
        }

        return cards;
    }
}
#endregion



#region 无尽模式
public class Matrix_10000 : Matrix
{
    private int m_Count = -1;
    public override List<Card> AddCards()
    {
        //将虚化方块实体化
        Field.Instance.CorporealCards();

        List<Card> cards    = new List<Card>();

        //获取3个空白格
        List<object> grids  = RandomUtility.Pick(3, Field.Instance.GetEmptyGrids());

        m_Count++;
        //目前2回合生成一个石块
        for (int i = 0; i < grids.Count; i++)
        {
            CardData data = null;
            if (i == 0 && m_Count == 2) {
                m_Count = 0;
                data    = GameFacade.Instance.DataCenter.GetCardData((int)_C.CARD.STONE);
            } else  {
                data    = m_Stage.Cards[RandomUtility.Random(0, m_Stage.Cards.Count)];
            }
            Grid grid   = grids[i] as Grid;
            Card card   = Field.Instance.PutCard(_C.CARD_STATE.GHOST, data, grid);

            cards.Add(card);
        }

        return cards;
    }
}
#endregion








//负责各关卡的特殊处理
public class Matrix
{
    protected Stage m_Stage;

    public void Init(Stage stage)
    {
        m_Stage = stage;
    }

    public virtual void FilterGrids()
    {
        m_Stage.GridJSONs.ForEach(grid_json => {
            var grid        = Field.Instance.GetGrid(grid_json.X, grid_json.Y);
            grid.IsValid    = grid_json.IsValid;
        });
    }


    //初始生成：读取配置
    public virtual void InitCards(int count = 3)
    {
        bool is_preset = false;

        m_Stage.GridJSONs.ForEach(grid_json => {
            if (grid_json.JellyID > 0) {
                is_preset   = true;

                var grid    = Field.Instance.GetGrid(grid_json.X, grid_json.Y);
                Field.Instance.PutCard(_C.CARD_STATE.NORMAL, GameFacade.Instance.DataCenter.GetCardData(grid_json.JellyID), grid);
            }
        });

        //已经预设好了
        if (is_preset) return;

        //没有预设，则随机生成(同色不相邻)
        List<object> grid_datas = Field.Instance.GetEmptyGrids();

        for (int i = 0; i < count; i++)
        {
            var data    = m_Stage.Cards[RandomUtility.Random(0, m_Stage.Cards.Count)];

            Grid grid   = grid_datas[RandomUtility.Random(0, grid_datas.Count)] as Grid;
            
            while (Field.Instance.GetSameCardNear(grid, data.ID).Count > 0)
            {
                grid   = grid_datas[RandomUtility.Random(0, grid_datas.Count)] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }

    }

    //默认
    public virtual List<Card> AddCards()
    {
        //将虚化方块实体化
        Field.Instance.CorporealCards();


        //添加虚化方块
        int min = 2;
        int max = 4;

        if (Field.Instance.Cards.Count <= 4) {
            min++;
            max++;
        }

        int count   = RandomUtility.Random(min, max);

        return Field.Instance.InitGhostCards(count);
    }

    public virtual void Dispose()
    {

    }
}





