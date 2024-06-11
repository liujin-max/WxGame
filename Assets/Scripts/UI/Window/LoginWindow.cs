using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    [SerializeField] private Text m_Coin;
    [SerializeField] private Text m_Food;


    [SerializeField] private Button m_BtnStage;
    

    void Awake()
    {
        m_BtnStage.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                //进入游戏
                NavigationController.GotoGame();
            });

            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }


    public void Init()
    {
        m_Coin.text = GameFacade.Instance.DataCenter.User.Coin.ToString();
        m_Food.text = GameFacade.Instance.DataCenter.User.Food.ToString();

        FlushCost();
    }

    public void FlushCost()
    {
        var stage_json = GameFacade.Instance.DataCenter.Level.GetStageJSON(GameFacade.Instance.DataCenter.User.Level + 1);
        int food = stage_json.Food;

        GameObject pivot = m_BtnStage.transform.Find("CostPivot").gameObject;
        if (food > 0) {
            pivot.gameObject.SetActive(true);

            if (GameFacade.Instance.DataCenter.User.Food >= food) 
            {
                pivot.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = food.ToString();
            }
            else pivot.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = Color.red + food.ToString();
        } else {
            pivot.SetActive(false);
        }
        
    }
}
