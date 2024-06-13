using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnReward;
    [SerializeField] Button m_BtnReturn;

    [SerializeField] Transform m_CoinPivot;


    // Start is called before the first frame update
    void Start()
    {
        //返回
        m_BtnReturn.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                
                NavigationController.GotoLogin();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            }); 
        });

        //激励广告：奖励翻倍
        m_BtnReward.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                m_BtnReward.gameObject.SetActive(false);

                GameFacade.Instance.DataCenter.User.UpdateCoin(Field.Instance.Stage.Coin);
                GameFacade.Instance.DataCenter.User.UpdateFood(Field.Instance.Stage.Food);

                EventManager.SendEvent(new GameEvent(EVENT.UI_UPDATECOIN));

                m_CoinPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Coin * 2);
            });
        });
    }

    public void Init()
    {
        m_CoinPivot.gameObject.SetActive(Field.Instance.Stage.Coin > 0);
        m_CoinPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Coin);


    }
}
