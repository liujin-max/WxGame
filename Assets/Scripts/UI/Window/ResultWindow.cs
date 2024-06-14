using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    [SerializeField] Transform m_ScorePivot;
    [SerializeField] Text m_Score;
    [SerializeField] Text m_SubScore;

    [SerializeField] Transform m_CoinPivot;

    [SerializeField] Button m_BtnReward;
    [SerializeField] Button m_BtnReturn;



    // Start is called before the first frame update
    void Start()
    {
        //返回
        m_BtnReturn.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Leave();
                
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

    public void Init(int score)
    {
        m_CoinPivot.gameObject.SetActive(Field.Instance.Stage.Coin > 0);
        m_CoinPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Coin);


        m_Score.text    = score.ToString();
        m_SubScore.text = score.ToString();
        m_ScorePivot.transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.4f);
    }
}
