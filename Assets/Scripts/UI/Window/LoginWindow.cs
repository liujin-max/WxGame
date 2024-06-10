using System.Collections;
using System.Collections.Generic;
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
    }
}
