using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropUseWindow : MonoBehaviour
{
    [SerializeField] Image m_Icon;
    [SerializeField] TextMeshProUGUI m_Description;

    [Header("按钮")]
    [SerializeField] private Button m_BtnClose;
    [SerializeField] private Button m_BtnCost;
    [SerializeField] private Button m_BtnVideo;
    [SerializeField] private Button m_BtnShare;


    private int m_Cost;
    private Action m_Callback;


    // Start is called before the first frame update
    void Start()
    {
        //花费金币购买
        m_BtnCost.onClick.AddListener(()=>{
            if (GameFacade.Instance.DataCenter.User.Coin < m_Cost) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "<sprite=0>不足"));
                return;
            }

            GameFacade.Instance.DataCenter.User.UpdateCoin(-m_Cost);
            EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECOIN));

            GameFacade.Instance.SoundManager.Load(SOUND.COIN);

            m_Callback();
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        //看广告给奖励
        m_BtnVideo.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
               m_Callback(); 
               GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        });

         //分享给奖励
        m_BtnShare.onClick.AddListener(()=>{
            Platform.Instance.SHARE("快来帮帮我！");

            m_Callback(); 
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        m_BtnClose.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    public void Init(int cost, string icon, string descritpion, Action callback)
    {
        m_Cost      = cost;
        m_Callback  = callback;

        m_Icon.sprite = Resources.Load<Sprite>("UI/Prop/" + icon);
        m_Icon.SetNativeSize();

        m_Description.text = descritpion;

        StringBuilder sb = new StringBuilder();
        if (GameFacade.Instance.DataCenter.User.Coin < m_Cost) sb.Append("<#FF0000>");
        sb.Append(cost);

        m_BtnCost.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = sb.ToString();
    }
}
