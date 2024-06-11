using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnReward;
    [SerializeField] Button m_BtnContinue;
    [SerializeField] Button m_BtnReturn;

    [SerializeField] Transform m_CoinPivot;
    [SerializeField] Transform m_FoodPivot;

    private CDTimer m_StarTimer = new CDTimer(0.1f);

    // Start is called before the first frame update
    void Start()
    {
        //下一关
        m_BtnContinue.onClick.AddListener(()=>{
            int level   = Field.Instance.Stage.ID + 1;
            var json    = GameFacade.Instance.DataCenter.Level.GetStageJSON(level);

            if (!GameFacade.Instance.DataCenter.Level.IsFoodEnough2Next(json)) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "体力不足"));
                return;
            }

            //扣除体力
            GameFacade.Instance.DataCenter.User.UpdateFood(-json.Food);

            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                Field.Instance.Enter(level);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });     
        });

        //返回
        m_BtnReturn.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                
                NavigationController.GotoLogin();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            }); 
        });

        //奖励翻倍
        m_BtnReward.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                m_BtnReward.gameObject.SetActive(false);

                GameFacade.Instance.DataCenter.User.UpdateCoin(Field.Instance.Stage.Coin);
                GameFacade.Instance.DataCenter.User.UpdateFood(Field.Instance.Stage.Food);

                m_CoinPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Coin * 2);
                m_FoodPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Food * 2);
            });
        });
    }

    public void Init()
    {
        m_CoinPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Coin);
        m_FoodPivot.Find("-/Value").GetComponent<NumberTransition>().SetValue(Field.Instance.Stage.Food);

        FlushUI();
    }

    void FlushUI()
    {
        var json = GameFacade.Instance.DataCenter.Level.GetStageJSON(Field.Instance.Stage.ID + 1);

        if (json == null) {
            m_BtnContinue.gameObject.SetActive(false);
            return;
        }

        m_BtnContinue.gameObject.SetActive(true);

        int food = json.Food;

        if (GameFacade.Instance.DataCenter.User.Food >= food)  {
            m_BtnContinue.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text = food.ToString();
        }
        else m_BtnContinue.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text = "<#FF0000>" + food.ToString();
    }

    void Update()
    {
        m_StarTimer.Update(Time.deltaTime);
        if (m_StarTimer.IsFinished() == true)
        {
            m_StarTimer.Reset(RandomUtility.Random(300, 700) / 1000.0f);


            
            float pos_x     = RandomUtility.Random(-50000, 50000) / 100.0f;
            float pos_y     = RandomUtility.Random(-25000, 25000) / 100.0f;
            Vector3 pos     = new Vector3(pos_x, 464 + pos_y, 0) / 100.0f;

            GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.SHINESTAR, pos);
        }
    }
}
