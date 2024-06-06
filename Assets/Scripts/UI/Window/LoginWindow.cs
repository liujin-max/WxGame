using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    [SerializeField] private Button m_BtnStage;

    void Awake()
    {
        m_BtnStage.onClick.AddListener(()=>{

            GameFacade.Instance.EffectManager.Load("Prefab/Effect/SceneSwitch", Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                //进入游戏
                NavigationController.GotoGame();
            });

            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

}
