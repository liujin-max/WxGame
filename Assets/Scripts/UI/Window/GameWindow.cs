using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : MonoBehaviour
{

    [SerializeField] private Transform m_ConditionPivot;
    [SerializeField] private Text m_Stage;
    [SerializeField] private NumberTransition m_Coin;

    [SerializeField] private GameObject m_StepPivot;
    [SerializeField] private Text m_Step;

    [SerializeField] private GameObject m_TimePivot;
    [SerializeField] private Text m_Time;

    [Header("按钮")]
    [SerializeField] private Button m_BtnSetting;
    [SerializeField] GameObject m_ButtonPivot;
    [SerializeField] private Button m_BtnTime;
    [SerializeField] private Button m_BtnStep;
    [SerializeField] private Button m_BtnShuffle;
    [SerializeField] private Button m_BtnRevoke;


    private List<ConditionItem> m_ConditionItems = new List<ConditionItem>();
    private ConditionItem new_condition_item(int order)
    {
        ConditionItem item;

        if (m_ConditionItems.Count > order) {
            item = m_ConditionItems[order];
        } else {
            item = GameFacade.Instance.UIManager.LoadItem("ConditionItem", m_ConditionPivot).GetComponent<ConditionItem>();
            m_ConditionItems.Add(item); 
        }

        item.Show(true);

        return item;
    }



    void Awake()
    {
        EventManager.AddHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);

        EventManager.AddHandler(EVENT.UI_UPDATECOIN,    OnReponseCoinUpdate);
        EventManager.AddHandler(EVENT.UI_UPDATESTEP,    OnReponseStepUpdate);
        EventManager.AddHandler(EVENT.UI_UPDATETIME,    OnReponseTimeUpdate);
    }

    void Start()
    {
        //设置
        m_BtnSetting.onClick.AddListener(()=>{
            Field.Instance.Pause();

            GameFacade.Instance.UIManager.LoadWindow("SettingWindow", UIManager.BOARD).GetComponent<SettingWindow>().SetCallback(()=>{
                Field.Instance.Resume();
            });
        });


        //添加时间
        m_BtnTime.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                Field.Instance.ad_add_time(60);
            });
        });

        //添加步数
        m_BtnStep.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                Field.Instance.ad_add_step(5);
            });
        });

        //打乱方块
        m_BtnShuffle.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                Field.Instance.ad_shuffle();
            });
        });

        //撤销操作
        m_BtnRevoke.onClick.AddListener(()=>{
            Platform.Instance.REWARD_VIDEOAD("", ()=>{
                Field.Instance.ad_revoke();
            });
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONENTERSTAGE,     OnReponseEnterStage);

        EventManager.DelHandler(EVENT.UI_UPDATECOIN,    OnReponseCoinUpdate);
        EventManager.DelHandler(EVENT.UI_UPDATESTEP,    OnReponseStepUpdate);
        EventManager.DelHandler(EVENT.UI_UPDATETIME,    OnReponseTimeUpdate);
        
    }

    public ConditionItem GetConditionItem(int id)
    {
        for (int i = 0; i < m_ConditionItems.Count; i++)  {
            var item = m_ConditionItems[i];
            if (item.Condition.ID == id && item.gameObject.activeSelf) 
                return item;
        }
        return null;
    }


    #region 监听事件
    private void OnReponseEnterStage(GameEvent @event)
    {
        m_ButtonPivot.SetActive(Field.Instance.Stage.ID > 1);

        m_Stage.text = Field.Instance.Stage.ID.ToString();
        m_Coin.ForceValue(GameFacade.Instance.DataCenter.User.Coin);

        m_StepPivot.SetActive(Field.Instance.Stage.NeedCheckStep());
        m_Step.text = Field.Instance.Stage.GetCurrentStep().ToString();
        m_Step.color= Color.white;

        m_TimePivot.SetActive(Field.Instance.Stage.NeedCheckTimer());
        m_Time.text = ToolUtility.Second2Minute(Mathf.CeilToInt(Field.Instance.Stage.GetCurrentTimer()));
        m_Time.color= Color.white;

        //初始化条件
        m_ConditionItems.ForEach(item => {item.Show(false);});
        for (int i = 0; i < Field.Instance.Stage.Conditions.Count; i++) {
            var condition = Field.Instance.Stage.Conditions[i];
            var item = new_condition_item(i);
            item.Init(condition);
        }
    }
    
    private void OnReponseCoinUpdate(GameEvent @event)
    {
        m_Coin.SetValue(GameFacade.Instance.DataCenter.User.Coin);
    }

    private void OnReponseStepUpdate(GameEvent @event)
    {
        bool will_shake = (bool)@event.GetParam(0);
        
        int step    = Field.Instance.Stage.GetCurrentStep();
        var color   = step <= 5 ? Color.red : Color.white;

        m_Step.text = step.ToString();
        m_Step.color= color;

        if (will_shake || step <= 5) 
        {
            m_Step.transform.DOShakePosition(0.4f, 5f, 15, 60);

            if (will_shake) {
                m_Step.color= Color.green;
                m_Step.DOColor(color, 0.5f);
            }
        }
    }

    private void OnReponseTimeUpdate(GameEvent @event)
    {
        bool will_shake = (bool)@event.GetParam(0);

        float second= Field.Instance.Stage.GetCurrentTimer();
        var color   = second <= 30 ? Color.red : Color.white;

        m_Time.text = ToolUtility.Second2Minute(Mathf.CeilToInt(second));
        m_Time.color= color;

        if (will_shake || second <= 30) 
        {
            m_Time.transform.DOShakePosition(0.4f, 5f, 15, 60);

            if (will_shake) {
                m_Time.color= Color.green;
                m_Time.DOColor(color, 0.5f);
            }
        }

    }

    #endregion
}
