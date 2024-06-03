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
    [SerializeField] private TextMeshProUGUI m_Stage;
    [SerializeField] private TextMeshProUGUI m_Step;



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
        EventManager.AddHandler(EVENT.ONINITGRID,       OnReponseInitGrids);
        EventManager.AddHandler(EVENT.ONADDCARD,        OnReponseAddCard);
        EventManager.AddHandler(EVENT.ONCARDMOVED,      OnReponseCardMoved);
        
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);
        EventManager.DelHandler(EVENT.ONINITGRID,       OnReponseInitGrids);
        EventManager.DelHandler(EVENT.ONADDCARD,        OnReponseAddCard);
        EventManager.DelHandler(EVENT.ONCARDMOVED,      OnReponseCardMoved);

        
    }


    void AddCard(Card card)
    {
        var item = GameFacade.Instance.UIManager.LoadItem("CardView", m_CardPivot).GetComponent<CardView>();
        item.transform.localPosition = card.Grid.Position;
        item.Init(card);
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

    private void OnReponseInitGrids(GameEvent @event)
    {
        m_GridItems.ForEach(item => {item.Show(false);});

        int count = 0;
        for (int i = 0; i < Field.Instance.Grids.GetLength(0); i++) {
            for (int j = 0; j < Field.Instance.Grids.GetLength(1); j++) {
                var grid = Field.Instance.Grids[i ,j];

                var item = new_grid_view(count);
                item.transform.localPosition = grid.Position;
                item.Init(grid);
                item.Show(grid.IsValid);

                count++;
            }
        }
    }
    
    private void OnReponseAddCard(GameEvent @event)
    {
        Card card = @event.GetParam(0) as Card;
        this.AddCard(card);
    }

    private void OnReponseCardMoved(GameEvent @event)
    {
        m_Step.text = Field.Instance.Stage.MoveStep.Current.ToString();
    }


    #endregion
}
