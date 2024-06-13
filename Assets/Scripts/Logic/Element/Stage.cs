using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡数据
public class Stage
{
    private StageJSON m_Data;

    public int ID {get{return m_Data.ID;}}
    public int Weight {get{return m_Data.Weight;}}
    public int Height {get{return m_Data.Height;}}
    public int Coin {get{return m_Data.Coin;}}
    public int Food {get{return m_Data.Food;}}

    public _C.MODE MODE {
        get {
            if (m_Data.ID >= 10000) return _C.MODE.ENDLESS;
            
            return _C.MODE.CHAPTER;
        }
    }


    private List<GridJSON> m_GridJSONs = new List<GridJSON>();
    private GridJSON[,] m_GridJSONDic;
    public List<GridJSON> GridJSONs {get {return m_Data.Grids;}}


    private List<Condition> m_Conditions = new List<Condition>();
    public List<Condition> Conditions {get{return m_Conditions;}}

    private List<CardData> m_Cards = new List<CardData>();
    private Dictionary<int, CardData> m_CardDic = new Dictionary<int, CardData>();
    public List<CardData> Cards {get{return this.m_Cards;}}

    //行动次数
    private Pair m_MoveStep;
    private CDTimer m_CountDownTimer;
    private Matrix m_Matrix;

    #region 配置关卡
    private static Dictionary<int, Func<Matrix>> m_classDictionary = new Dictionary<int, Func<Matrix>> {
        {  1, () => new Matrix_1()},
        { 14, () => new Matrix_14()},
        { 15, () => new Matrix_15()},
        { 16, () => new Matrix_16()},
        { 18, () => new Matrix_18()},

        {10000, () => new Matrix_10000()},   //无尽模式
    };
    #endregion

    public Stage(StageJSON stageData)
    {
        m_Data  = stageData;

        m_MoveStep = new Pair(m_Data.Step, m_Data.Step);
        m_CountDownTimer = new CDTimer(m_Data.Time, true);


        if (m_classDictionary.ContainsKey(this.ID)) {
            m_Matrix = m_classDictionary[this.ID]();
        } else {
            m_Matrix = new Matrix();
        }
        m_Matrix.Init(this);

        ParseJSONs();
        ParseCardPool();
        ParseCondition();
    }

    public void Dispose()
    {
        m_Matrix.Dispose();
    }

    void ParseJSONs()
    {
        m_GridJSONDic = new GridJSON[Weight, Height];

        m_Data.Grids.ForEach(g => {
            m_GridJSONs.Add(g);
            m_GridJSONDic[g.X, g.Y] = g;
        });
    }

    void ParseCardPool()
    {
        m_Cards.Clear();
        m_CardDic.Clear();

        foreach (var card_id in m_Data.CardPool) {
            CardData card_data = GameFacade.Instance.DataCenter.GetCardData(card_id);
            m_Cards.Add(card_data);
            m_CardDic[card_id] = card_data;
        }
    }


    void ParseCondition()
    {
        m_Conditions.Clear();

        foreach (var item in m_Data.Conditions) {
            string[] conditions = item.Split(':');
            var condition = new Condition(Convert.ToInt32(conditions[0]), Convert.ToInt32(conditions[1]));
            m_Conditions.Add(condition);
        }
    }



    public GridJSON GetGridJSON(int x, int y)
    {
        return m_GridJSONDic[x, y];
    }

    public bool NeedCheckStep()
    {
        return m_MoveStep.Total > 0;
    }

    public bool IsStepClear()
    {
        return m_MoveStep.IsClear();
    }

    public void UpdateMoveStep(int step)
    {
        m_MoveStep.UpdateCurrent(step);
    }

    public void SetMoveStep(int step)
    {
        m_MoveStep.SetCurrent(step);
    }

    public int GetCurrentStep()
    {
        return m_MoveStep.Current;
    }

    public bool NeedCheckTimer()
    {
        return m_CountDownTimer.Duration > 0;
    }

    public bool IsTimerClear()
    {
        return m_CountDownTimer.IsFinished();
    }

    public void UpdateCountDown(float time)
    {
        m_CountDownTimer.Update(time);
    }

    public void SetCountDown(float time)
    {
        m_CountDownTimer.SetCurrent(time);
    }

    public float GetCurrentTimer()
    {
        return m_CountDownTimer.Current;
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

}
