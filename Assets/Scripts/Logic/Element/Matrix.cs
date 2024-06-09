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

    public override void InitCards(int count)
    {
        CardData cardData = GameFacade.Instance.DataCenter.GetCardData(10001);

        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(1, 2));
        Field.Instance.PutCard(_C.CARD_STATE.NORMAL, cardData, Field.Instance.GetGrid(5, 2)).Dragable = false;
    }

    public override List<Card> AddCards(int random_count = -1)
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



        return add_cards; 
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
            
            while (Field.Instance.IsSameCardNear(grid, data.ID))
            {
                rand    = RandomUtility.Random(0, grid_datas.Count);
                grid    = grid_datas[rand] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }
    }

    public override List<Card> AddCards(int random_count = -1)
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
            
            while (Field.Instance.IsSameCardNear(grid, data.ID))
            {
                rand    = RandomUtility.Random(0, grid_datas.Count);
                grid    = grid_datas[rand] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }
    }

    public override List<Card> AddCards(int random_count = -1)
    {
        return new List<Card>();
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
            
            while (Field.Instance.IsSameCardNear(grid, data.ID))
            {
                grid   = grid_datas[RandomUtility.Random(0, grid_datas.Count)] as Grid;
            }

            Field.Instance.PutCard(_C.CARD_STATE.NORMAL, data, grid);

            grid_datas.Remove(grid);
        }

    }

    //默认
    public virtual List<Card> AddCards(int random_count = -1)
    {
        //将虚化方块实体化
        Field.Instance.GhostCards.ForEach(card => {
            if (card.Grid.IsEmpty == true) {
                Field.Instance.PutCard(_C.CARD_STATE.NORMAL, card, card.Grid);
            } else {
                Debug.LogError("实体化时，当前坐标已经有方块了：" + card.Grid.X + ", " + card.Grid.Y);
            }
        });
        Field.Instance.GhostCards.Clear();


        //添加虚化方块
        int count = random_count == -1 ? RandomUtility.Random(2, 4) : random_count;

        return Field.Instance.PutGhostCards(count);
    }

    public virtual void Dispose()
    {

    }
}





