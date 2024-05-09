using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TipWindow : MonoBehaviour
{
    [SerializeField] GameObject c_TipPivot;
    [SerializeField] GameObject c_ACHPivot;
    [SerializeField] GameObject c_RankPivot;

    List<ACHPopItem> m_ACHItems = new List<ACHPopItem>();


    private Sequence m_Sequence = null;

    private ACHPopItem new_ach_item()
    {
        for (int i = 0; i < m_ACHItems.Count; i++) {
            var item = m_ACHItems[i];
            if (item.IsShow() != true) {
                return item;
            }
        }

        GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/ACHPopItem", c_ACHPivot.transform);
        var it = obj.GetComponent<ACHPopItem>();
        m_ACHItems.Add(it);

        return it;
    }

    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_ACHIEVEMENTPOP,    OnReponsePopupACH);
        EventManager.AddHandler(EVENT.UI_RANKINGUP,         OnReponseRankUp);
    }

    // Start is called before the first frame update
    void Start()
    {
        c_TipPivot.SetActive(false);
    }

    public void FlyTip(string text)
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.MEDIUM);
        GameFacade.Instance.SoundManager.Load(SOUND.TIP);
        
        c_TipPivot.SetActive(true);
        c_TipPivot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        

        if (m_Sequence != null) {
            m_Sequence.Kill();
        }
        var group   = c_TipPivot.GetComponent<CanvasGroup>();
        group.alpha = 1;

        m_Sequence = DOTween.Sequence();
        m_Sequence.Join(c_TipPivot.transform.DOShakePosition(0.25f, new Vector3(10, 0, 0), 40, 50));
        m_Sequence.Join(group.DOFade(1f, 1.5f));
        m_Sequence.Append(group.DOFade(0f, 0.5f));
        m_Sequence.Play();
    }

    void OnReponsePopupACH(GameEvent gameEvent)
    {
        Achievement ach = (Achievement)gameEvent.GetParam(0);

        var item = new_ach_item();
        item.Show(true);
        item.Init(ach);
    }

    void OnReponseRankUp(GameEvent gameEvent)
    {
        RankData rankData = (RankData)gameEvent.GetParam(0);

        GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RankUPItem", c_RankPivot.transform);
        var it = obj.GetComponent<RankUPItem>();
        it.Init(rankData);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_ACHIEVEMENTPOP,    OnReponsePopupACH);
        EventManager.DelHandler(EVENT.UI_RANKINGUP,         OnReponseRankUp);
    }
}
