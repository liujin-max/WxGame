using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private TextMeshProUGUI m_Tip;

    [SerializeField] private Button m_BtnReceive;



    private Task m_Task;

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_UPDATETASK,    OnFlushUI);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_UPDATETASK,    OnFlushUI);
    }


    // Start is called before the first frame update
    void Start()
    {
        m_BtnReceive.onClick.AddListener(()=>{
            if (m_Task.STATE != _C.TASK_STATE.FINISH) return;

            GameFacade.Instance.DataCenter.Daily.ReceiveTask(m_Task);
        });
    }

    public void Init(Task task)
    {
        m_Task = task;

        m_Title.text = task.Name;
        m_Description.text = task.Description;

        FlushUI();
    }

    void FlushUI()
    {
        m_Tip.gameObject.SetActive(false);
        m_BtnReceive.gameObject.SetActive(false);

        if (m_Task.STATE == _C.TASK_STATE.NONE)
        {
            m_Tip.gameObject.SetActive(true);
            m_Tip.text = "进行中";
        }   
        else if (m_Task.STATE == _C.TASK_STATE.FINISH)
        {
            m_BtnReceive.gameObject.SetActive(true);
        }
        else
        {
            m_Tip.gameObject.SetActive(true);
            m_Tip.text = "已完成";
        }
    }


    #region 监听事件
    private void OnFlushUI(GameEvent @event)
    {
        var task = @event.GetParam(0) as Task;

        if (task == m_Task) FlushUI();
    }
    #endregion
}
