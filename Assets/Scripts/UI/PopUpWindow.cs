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

    private Action m_ConfirmCallback;
    private Action m_CancelCallback;

    void Awake()
    {
        c_BtnConfirm.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (m_ConfirmCallback != null) {
                m_ConfirmCallback();
            }
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        c_BtnCancel.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (m_CancelCallback != null) {
                m_CancelCallback();
            }
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    public void Init(string des, Action confirm_callback, Action cancel_callback)
    {
        c_Description.text  = des;

        m_ConfirmCallback   = confirm_callback;
        m_CancelCallback    = cancel_callback;
    }
}
