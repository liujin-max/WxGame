using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//负责记录每一回合 场上的状态
//包括 所有方块的位置和状态
//当前步数、当前倒计时
//当前目标完成情况
public class History 
{
    private int m_Turn;
    private int m_Step;
    private float m_Time;

    private List<Card> m_CardRecords = new List<Card>();
    private Dictionary<int, int> m_Conditions = new Dictionary<int, int>();

    public History(int turn)
    {
        m_Turn  = turn;
    }

    //记录
    public void Record()
    {
        //步骤、倒计时
        m_Step  = Field.Instance.Stage.GetCurrentStep();
        m_Time  = Field.Instance.Stage.GetCurrentTimer();

        //记录卡牌
        Field.Instance.Cards.ForEach(c => {
            Card card = new Card(GameFacade.Instance.DataCenter.GetCardData(c.ID));
            card.Grid = c.Grid;
            card.STATE= c.STATE;

            m_CardRecords.Add(card);
        });

        Field.Instance.GhostCards.ForEach(c => {
            Card card = new Card(GameFacade.Instance.DataCenter.GetCardData(c.ID));
            card.Grid = c.Grid;
            card.STATE= c.STATE;

            m_CardRecords.Add(card);
        });

        //记录目标完成情况
        Field.Instance.Stage.Conditions.ForEach(condition => {
            m_Conditions[condition.ID] = condition.Count.Current;
        });
    }
    
    //回溯
    public void Revoke()
    {
        Field.Instance.Turn = m_Turn;

        //清空当前场上的方块
        Field.Instance.Cards.ForEach((card) => {
            card.Dispose();
        });
        Field.Instance.Cards.Clear();

        Field.Instance.GhostCards.ForEach((card) => {
            card.Dispose();
        });
        Field.Instance.GhostCards.Clear();

        //加载方块
        m_CardRecords.ForEach(c => {
            Field.Instance.PutCard(c.STATE, c.Data, c.Grid);
        });

        //加载步数、倒计时
        Field.Instance.Stage.SetMoveStep(m_Step);
        Field.Instance.Stage.SetCountDown(m_Time);

        //加载目标完成情况
        foreach (var item in m_Conditions)
        {
            var condition = Field.Instance.Stage.GetCondition(item.Key);
            condition.SetCount(item.Value);

            EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECONDITION, condition));
        }

        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATESTEP, true));
        EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATETIME, true));
    }
}
