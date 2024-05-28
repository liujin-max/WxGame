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
    void Awake()
    {
        m_BtnTop.onClick.AddListener(()=>{
            Field.Instance.MoveTop();

            MoveCardItems();
        });

        m_BtnDown.onClick.AddListener(()=>{
            Field.Instance.MoveDown();

            MoveCardItems();
        });

        m_BtnLeft.onClick.AddListener(()=>{
            Field.Instance.MoveLeft();

            MoveCardItems();
        });

        m_BtnRight.onClick.AddListener(()=>{
            Field.Instance.MoveRight();

            MoveCardItems();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveCardItems()
    {
        for (int i = m_CardItems.Count - 1; i >= 0; i--)
        {
            var item = m_CardItems[i];

            item.transform.DOLocalMove(item.Card.Grid.GetPosition(), 0.3f).OnComplete(()=>{
                if (item.Card.IsEliminate == true) {
                    Destroy(item.gameObject);
                    m_CardItems.Remove(item);
                }
            });
        }
    }

    public void Init()
    {
        Field.Instance.Cards.ForEach(card => {
            var item = GameFacade.Instance.UIManager.LoadItem("CardView", m_CardPivot).GetComponent<CardView>();
            item.transform.localPosition = card.Grid.GetPosition();
            item.Init(card);
            m_CardItems.Add(item);
        });
    }
}
