using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Transform m_ConditionPivot;
    [SerializeField] private Transform m_CardPivot;
    [SerializeField] private Text m_Stage;
    [SerializeField] private Text m_Coin;

    [SerializeField] private GameObject m_StepPivot;
    [SerializeField] private Text m_Step;

    [SerializeField] private GameObject m_TimePivot;
    [SerializeField] private Text m_Time;




    private List<ConditionItem> m_ConditionItems = new List<ConditionItem>();
    private ConditionItem new_condition_item(int order)
    {
        ConditionItem item;

        if (m_ConditionItems.Count > order) {
            item = m_ConditionItems[order];
        } else {
            item = GameFacade.Instance.UIManager.LoadItem("ConditionItem", m_ConditionPivot).GetComponent<ConditionItem>();
            m_ConditionItems.Add(item); 
        }

        item.Show(true);

        return item;
    }



    void Awake()
    {
        EventManager.AddHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);

        EventManager.AddHandler(EVENT.UI_UPDATESTEP,    OnReponseStepUpdate);
        EventManager.AddHandler(EVENT.UI_UPDATETIME,    OnReponseTimeUpdate);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);

        EventManager.DelHandler(EVENT.UI_UPDATESTEP,    OnReponseStepUpdate);
        EventManager.DelHandler(EVENT.UI_UPDATETIME,    OnReponseTimeUpdate);
        
    }




    #region 监听事件
    private void OnReponseEnterStage(GameEvent @event)
    {
        m_Stage.text = Field.Instance.Stage.ID.ToString();
        m_Coin.text = GameFacade.Instance.DataCenter.User.Coin.ToString();

        m_StepPivot.SetActive(Field.Instance.Stage.NeedCheckStep());
        m_Step.text = Field.Instance.Stage.GetCurrentStep().ToString();
        m_Step.color= Color.white;

        m_TimePivot.SetActive(Field.Instance.Stage.NeedCheckTimer());
        m_Time.text = ToolUtility.Second2Minute(Mathf.CeilToInt(Field.Instance.Stage.GetCurrentTimer()));
        m_Time.color= Color.white;

        //初始化条件
        m_ConditionItems.ForEach(item => {item.Show(false);});
        for (int i = 0; i < Field.Instance.Stage.Conditions.Count; i++) {
            var condition = Field.Instance.Stage.Conditions[i];
            var item = new_condition_item(i);
            item.Init(condition);
        }
    }
    

    private void OnReponseStepUpdate(GameEvent @event)
    {
        int step    = Field.Instance.Stage.GetCurrentStep();
        m_Step.text = step.ToString();
        m_Step.color= step <= 5 ? Color.red : Color.white;

        if (step <= 5) 
        {
            m_Step.transform.DOShakePosition(0.4f, 5f, 15, 60);
        }
    }

    private void OnReponseTimeUpdate(GameEvent @event)
    {
        float second= Field.Instance.Stage.GetCurrentTimer();
        m_Time.text = ToolUtility.Second2Minute(Mathf.CeilToInt(second));
        m_Time.color= second <= 30 ? Color.red : Color.white;

        if (second <= 30) 
        {
            m_Time.transform.DOShakePosition(0.4f, 5f, 15, 60);
        }

    }

    #endregion
}
