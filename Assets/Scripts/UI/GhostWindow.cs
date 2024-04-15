using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// using UnityEngine.UIElements;
public class GhostWindow : MonoBehaviour
{
    [SerializeField] private Transform c_Pivot;
    [SerializeField] private Button c_BtnSelect;
    [SerializeField] private Button c_BtnCancel;
    [SerializeField] private Button c_BtnRefresh;
    [SerializeField] private GameObject c_ButtonPivot;
    [SerializeField] private GameObject c_DescriptionPivot;
    [SerializeField] private TextMeshProUGUI c_Tip;

    private GhostItem m_SelectGhost = null;
    private List<GhostItem> m_GhostItems = new List<GhostItem>();

    private CDTimer m_CDTimer = new CDTimer(0.6f);

    void Start()
    {
        c_DescriptionPivot.SetActive(false);

        c_BtnRefresh.gameObject.SetActive(false);
        c_BtnSelect.gameObject.SetActive(false);
        c_BtnCancel.gameObject.SetActive(false);

        c_BtnSelect.onClick.AddListener(()=> {
            var flag = GameFacade.Instance.Game.ComplextBall(m_SelectGhost.m_Event);
            if (flag == true) {
                GameFacade.Instance.SoundManager.Load(SOUND.COST);

                List<ComplextEvent> events = GameFacade.Instance.Game.GenerateEvents();
                if (events != null) {
                    this.Init(events);
                }
            }
        });

        c_BtnCancel.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (GameFacade.Instance.Game.Balls.Count > 0) {
                GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_IDLE);
            } else {
               GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_END);
            }
        });

        c_BtnRefresh.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            List<ComplextEvent> events = GameFacade.Instance.Game.RefreshEvents();
            if (events != null) {
                GameFacade.Instance.SoundManager.Load(SOUND.COST);
                this.Init(events);
            }
        });


    }

    public void Init(List<ComplextEvent> events)
    {
        FlushUI();
        InitEvents(events);
    }

    void FlushUI()
    {
        c_Tip.text = string.Format("消耗 <sprite=0>（拥有{0}个）合成弹珠", GameFacade.Instance.Game.Glass);

        if (GameFacade.Instance.Game.m_Coin >= GameFacade.Instance.Game.RefreshCoin) {
            c_BtnRefresh.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = GameFacade.Instance.Game.RefreshCoin + " <sprite=1>";
        } else {
            c_BtnRefresh.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _C.REDCOLOR + GameFacade.Instance.Game.RefreshCoin + " <sprite=1>";
        }
        
    }

    GhostItem new_ghost_item(int order)
    {
        GhostItem item = null;
        if (m_GhostItems.Count > order){
            item = m_GhostItems[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/GhostItem", c_Pivot);
            item = obj.GetComponent<GhostItem>();

            m_GhostItems.Add(item);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    public void InitEvents(List<ComplextEvent> events)
    {
        if (m_SelectGhost != null) {
            m_SelectGhost.Select(false);
        }
        m_SelectGhost = null;

        foreach (var item in m_GhostItems) {
            item.gameObject.SetActive(false);
        }

        c_DescriptionPivot.SetActive(false);


        int count = events.Count;

        for (int i = 0; i < events.Count; i++)
        {
            var evt = events[i];
            GhostItem item = new_ghost_item(i);
            item.transform.localPosition = new Vector3((i - ((events.Count - 1) / 2.0f)) * 290, 0, 0);
            item.Init(evt);

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.BUBBLE);
                
                if (m_SelectGhost != null) {
                    m_SelectGhost.Select(false);
                }

                m_SelectGhost = item;
                m_SelectGhost.Select(true);

                c_BtnSelect.gameObject.SetActive(true);

                this.ShowDescription(evt);
            });
        }
    }

    void ShowDescription(ComplextEvent evt)
    {
        c_DescriptionPivot.SetActive(true);
        c_DescriptionPivot.transform.position = m_SelectGhost.transform.position + new Vector3(0, 2.5f, 0);


        var des_text = c_DescriptionPivot.transform.Find("Text").GetComponent<ShakeText>();


        if (evt.EventType == _C.COMPLEXTEVEMT.GLASS)
        {
            des_text.SetText("<#3297FF>碎片：</color>合成弹珠需要的材料");
        }
        else
        {
            var ball = m_SelectGhost.Ball;
            des_text.SetText(string.Format("<#3297FF>{0}<#FF6631>({1})</color>：</color>{2}", ball.Name, ball.m_Demage.ToNumber(), ball.GetDescription()));
        }
    }

    void Update()
    {
        if (m_CDTimer == null) return;

        m_CDTimer.Update(Time.deltaTime);
        if (m_CDTimer.IsFinished() == true)  {
            m_CDTimer = null;

            c_BtnRefresh.gameObject.SetActive(true);
            c_BtnCancel.gameObject.SetActive(true);
        }
    }
}
