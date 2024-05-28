using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Card m_Card;
    public Card Card { get { return m_Card;}}

    [SerializeField] private Image m_Icon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Card card)
    {
        m_Card = card;

        m_Icon.sprite = Resources.Load<Sprite>("UI/Card/" + card.ID);
        m_Icon.SetNativeSize();
    }
}
