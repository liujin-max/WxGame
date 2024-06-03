using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    private List<int> m_Cards = new List<int>();
    public List<int> Cards {get{return this.m_Cards;}}

    //行动次数
    public Pair MoveStep;

    public Stage(StageData stageData)
    {
        m_Data = stageData;

        MoveStep = new Pair(m_Data.Step, m_Data.Step);

        ParseCardPool();
        ParseCondition();

        EventManager.AddHandler(EVENT.ONCARDBROKEN,   OnCardBroken);
    }

    public void Dispose()
    {
        EventManager.DelHandler(EVENT.ONCARDBROKEN,   OnCardBroken);
    }

    void ParseCardPool()
    {
        m_Cards.Clear();

        foreach (var id in m_Data.Cards.Split('|')) {
            m_Cards.Add(Convert.ToInt32(id));
        }
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


    #region 监听事件
    private void OnCardBroken(GameEvent @event)
    {
        var card = @event.GetParam(0) as Card;
        this.Collect(card.ID, 1);
        
        // if (this.IsConformTo(card.ID) == true) {

        // }
    }
    #endregion
}
