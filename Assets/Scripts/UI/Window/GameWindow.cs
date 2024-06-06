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
    [SerializeField] private Text m_Step;



    private List<GridView> m_GridItems = new List<GridView>();
    private GridView new_grid_view(int order)
    {
        GridView gridView;

        if (m_GridItems.Count > order) {
            gridView = m_GridItems[order];
        } else {
            gridView = GameFacade.Instance.UIManager.LoadItem("GridView", m_CardPivot).GetComponent<GridView>();
            m_GridItems.Add(gridView); 
        }

        gridView.Show(true);

        return gridView;
    }


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
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnReponseCardMoved);
        
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnReponseCardMoved);

        
    }




    #region 监听事件
    private void OnReponseEnterStage(GameEvent @event)
    {
        m_Stage.text    = Field.Instance.Stage.Name;
        m_Step.text     = Field.Instance.Stage.MoveStep.Current.ToString();

        //初始化条件
        m_ConditionItems.ForEach(item => {item.Show(false);});
        for (int i = 0; i < Field.Instance.Stage.Conditions.Count; i++) {
            var condition = Field.Instance.Stage.Conditions[i];
            var item = new_condition_item(i);
            item.Init(condition);
        }
    }
    

    private void OnReponseCardMoved(GameEvent @event)
    {
        m_Step.text = Field.Instance.Stage.MoveStep.Current.ToString();
    }


    #endregion
}
