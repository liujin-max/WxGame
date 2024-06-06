using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//关卡数据
public class Stage
{
    private StageData m_Data;

    public int ID {get{return m_Data.ID;}}
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

    // public CardData GetCardData(int card_id)
    // {
    //     CardData cardData;
    //     if (m_CardDic.TryGetValue(card_id, out cardData)) {
    //         return cardData;
    //     }

    //     return null;
    // }

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
