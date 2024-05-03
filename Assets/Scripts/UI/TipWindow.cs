using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;

public class TipWindow : MonoBehaviour
{
    [SerializeField] GameObject c_TipPivot;
    [SerializeField] GameObject c_ACHPivot;

    [SerializeField] List<ACHPopItem> m_ACHItems = new List<ACHPopItem>();

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
        GameFacade.Instance.EventManager.AddHandler(EVENT.UI_ACHIEVEMENTPOP,    OnReponsePopupACH);
    }

    // Start is called before the first frame update
    void Start()
    {
        c_TipPivot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlyTip(string text)
    {
        Platform.Instance.VIBRATE(_C.VIBRATELEVEL.MEDIUM);
        
        c_TipPivot.SetActive(true);
        c_TipPivot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
        c_TipPivot.GetComponent<Animation>().Play("FlyTip");
    }

    void OnReponsePopupACH(GameEvent gameEvent)
    {
        Achievement ach = (Achievement)gameEvent.GetParam(0);

        var item = new_ach_item();
        item.Show(true);
        item.Init(ach);
    }

    void OnDestroy()
    {
        GameFacade.Instance.EventManager.DelHandler(EVENT.UI_ACHIEVEMENTPOP,    OnReponsePopupACH);
    }
}
