using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CB;

public class RankItem : MonoBehaviour
{
    [SerializeField] private Transform c_HeadPivot;
    [SerializeField] private Text c_Order;
    [SerializeField] private Text c_Nickname;
    [SerializeField] private Text c_Score;

    private HeadItem m_HeadItem = null;

    public void Init(RankData rankData)
    {
        InitHead(rankData.Head);    

        c_Order.text    = rankData.Order.ToString();
        c_Nickname.text = rankData.Name;
        c_Score.text    = string.Format("{0} 层", rankData.Score);
    }

    void InitHead(string head_url)
    {
        if (m_HeadItem == null) {
            m_HeadItem = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", c_HeadPivot).GetComponent<HeadItem>();
        } 
        m_HeadItem.Init(head_url);
    }
}
