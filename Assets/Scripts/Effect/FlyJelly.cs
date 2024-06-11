using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyJelly : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_Sprite;


    public void Init(Card card)
    {
        m_Sprite.sprite  = Resources.Load<Sprite>("UI/Element/jelly_" + card.ID);

    }
}
