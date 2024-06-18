using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Tip;

    [Header("按钮")]
    [SerializeField] private Button m_BtnClose;
    [SerializeField] private Button m_BtnVideo;

    // Start is called before the first frame update
    void Start()
    {
        m_BtnClose.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        m_BtnVideo.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                GameFacade.Instance.DataCenter.User.UpdateFood(_C.DEFAULT_FOOD);

                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATEFOOD));
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        });


        FlushTip();
    }

    // Update is called once per frame
    void Update()
    {
        FlushTip();
    }

    void FlushTip()
    {
        int second = GameFacade.Instance.DataCenter.User.GetFoodTimer();

        m_Tip.text = ToolUtility.Second2Minute(second);
    }
}
