using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI c_Description;
    [SerializeField] private Button c_BtnConfirm;
    [SerializeField] private Button c_BtnCancel;
    [SerializeField] private Button c_BtnVideo;
    [SerializeField] private Button c_BtnMoney;

    private Action m_ConfirmCallback;
    private Action m_VideoCallback;
    private Action m_CancelCallback;

    void Awake()
    {
        Platform.Instance.BANNER_VIDEOAD("adunit-0a5910ddc759e7d3", true);

        c_BtnConfirm.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (m_ConfirmCallback != null) {
                m_ConfirmCallback();
            }
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        c_BtnVideo.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            Platform.Instance.REWARD_VIDEOAD("adunit-0f86c93b24547757", ()=>{
                if (m_VideoCallback != null) {
                    m_VideoCallback();
                }
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        });

        //暂时隐藏
        c_BtnMoney.gameObject.SetActive(false);
        c_BtnMoney.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            Platform.Instance.REWARD_VIDEOAD("adunit-69f3ac167e9aae19", ()=>{
                GameFacade.Instance.Game.UpdateCoin(3, false);

                for (int i = 0; i < 3; i++) {
                    var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, c_BtnMoney.transform.position);
                    e.GetComponent<FlyCoin>().Fly(0.1f * i); 
                }
            });
        });

        c_BtnCancel.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (m_CancelCallback != null) {
                m_CancelCallback();
            }
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    void OnDestroy()
    {
        Platform.Instance.BANNER_VIDEOAD("adunit-0a5910ddc759e7d3", false);
    }

    public void Init(string des, Action confirm_callback, Action video_callback, Action cancel_callback)
    {
        c_Description.text  = des;

        m_ConfirmCallback   = confirm_callback;
        m_VideoCallback     = video_callback;
        m_CancelCallback    = cancel_callback;
    }

    public void ShowVideo(bool flag)
    {
        c_BtnVideo.gameObject.SetActive(flag);
    }
}
