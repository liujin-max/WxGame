using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Transform m_CardPivot;

    [SerializeField] private Button m_BtnTop;
    [SerializeField] private Button m_BtnDown;
    [SerializeField] private Button m_BtnLeft;
    [SerializeField] private Button m_BtnRight;


    private List<CardView> m_CardItems = new List<CardView>();
    private List<GridView> m_GridItems = new List<GridView>();

    private CardView m_CurCardItem = null;

    void Awake()
    {
        m_BtnTop.onClick.AddListener(()=>{

        });

        m_BtnDown.onClick.AddListener(()=>{

        });

        m_BtnLeft.onClick.AddListener(()=>{

        });

        m_BtnRight.onClick.AddListener(()=>{

        });

        EventManager.AddHandler(EVENT.UI_DESTROYCARD,   OnReponseDestroyCard);

        EventManager.AddHandler(EVENT.ONADDCARD,        OnReponseAddCard);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_DESTROYCARD,   OnReponseDestroyCard);

        EventManager.DelHandler(EVENT.ONADDCARD,        OnReponseAddCard);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        m_CardItems.Add(item);
    }


    #region 监听事件
    private void OnReponseDestroyCard(GameEvent @event)
    {
        List<CardView> _Removes = new List<CardView>();

        m_CardItems.ForEach(item => {
            if (item.Card.IsEliminate == true) {
                _Removes.Add(item);
            }
        });

        _Removes.ForEach(item => {
            Destroy(item.gameObject);
            m_CardItems.Remove(item);
        });
    }

    private void OnReponseAddCard(GameEvent @event)
    {
        Card card = @event.GetParam(0) as Card;
        this.AddCard(card);
    }
    #endregion
}
