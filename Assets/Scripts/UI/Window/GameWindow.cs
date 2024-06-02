using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Transform m_CardPivot;




    private List<GridView> m_GridItems = new List<GridView>();


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONADDCARD,        OnReponseAddCard);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONADDCARD,        OnReponseAddCard);
    }

    public void Init()
    {
        InitGrids();
        InitCards();
    }

    void InitGrids()
    {
        for (int i = 0; i < _C.DEFAULT_WEIGHT; i++) {
            for (int j = 0; j < _C.DEFAULT_HEIGHT; j++) {
                var grid = Field.Instance.Grids[i ,j];

                var item = GameFacade.Instance.UIManager.LoadItem("GridView", m_CardPivot).GetComponent<GridView>();
                item.transform.localPosition = grid.GetPosition();
                item.Init(grid);
                m_GridItems.Add(item); 
            }
        }
    }

    void InitCards()
    {
        Field.Instance.Cards.ForEach(card => {
            this.AddCard(card);
        });
    }

    void AddCard(Card card)
    {
        var item = GameFacade.Instance.UIManager.LoadItem("CardView", m_CardPivot).GetComponent<CardView>();
        item.transform.localPosition = card.Grid.GetPosition();
        item.Init(card);
    }


    #region 监听事件

    private void OnReponseAddCard(GameEvent @event)
    {
        Card card = @event.GetParam(0) as Card;
        this.AddCard(card);
    }
    #endregion
}
