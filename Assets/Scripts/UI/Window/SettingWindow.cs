using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingWindow : MonoBehaviour
{
    [SerializeField] Button c_Mask;
    [SerializeField] Slider c_MusicSlider;
    [SerializeField] Slider c_SoundSlider;
    [SerializeField] Toggle c_VibrateToggle;

    [SerializeField] GameObject m_ButtonPivot;
    [SerializeField] Button m_BtnReturn;
    [SerializeField] Button m_BtnContinue;

    [SerializeField] GameObject m_Tip;

    private Action m_Callback;

    void Awake()
    {
        // Platform.Instance.BANNER_VIDEOAD("adunit-572e9d91851655e6", true);

        c_Mask.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback();
            
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        c_MusicSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.MusicVolume = value;
        });

        c_SoundSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.SoundVolume = value;
        });

        c_VibrateToggle.onValueChanged.AddListener((flag)=>{
            GameFacade.Instance.SystemManager.VibrateFlag = flag;
        });


        //继续游戏
        m_BtnContinue.onClick.AddListener(()=>{
            if (m_Callback != null) m_Callback();
            
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        //返回主界面
        m_BtnReturn.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Leave();
                
                NavigationController.GotoLogin();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            }); 
        });
    }


    void OnDestroy()
    {
        // Platform.Instance.BANNER_VIDEOAD("adunit-572e9d91851655e6", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        c_MusicSlider.value     = GameFacade.Instance.SystemManager.MusicVolume;
        c_SoundSlider.value     = GameFacade.Instance.SystemManager.SoundVolume;

        c_VibrateToggle.isOn    = GameFacade.Instance.SystemManager.VibrateFlag;
    }

    public void SetCallback(Action callback)
    {
        m_Callback = callback;
    }

    public void ShowButton(bool flag)
    {
        m_ButtonPivot.SetActive(flag);
        m_Tip.SetActive(!flag);
    }
}
