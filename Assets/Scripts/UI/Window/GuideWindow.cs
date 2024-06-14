using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideWindow : MonoBehaviour
{
    [SerializeField] GameObject m_Guide1;
    [SerializeField] GameObject m_Guide2;

    private Dictionary<int, GameObject> m_Guides = new Dictionary<int, GameObject>();
    void Awake()
    {
        m_Guides[1] = m_Guide1;
        m_Guides[2] = m_Guide2;

        m_Guide1.SetActive(false);
        m_Guide2.SetActive(false);


        //适配遮罩高度
        // var seat_pivot = GameFacade.Instance.Game.GameUI.SeatPivot;
        // var pos = new Vector3(seat_pivot.transform.position.x * 100, seat_pivot.transform.position.y * 100 + 5, 0);
        // c_GuideBall.transform.GetComponent<UIMaskUtility>().SetCenter(pos);

        EventManager.AddHandler(EVENT.UI_SHOWGUIDE,     OnShowGuide);
    }


    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_SHOWGUIDE,     OnShowGuide);
    }

    private void OnShowGuide(GameEvent @event)
    {
        int index = (int)@event.GetParam(0);
        bool flag = (bool)@event.GetParam(1);

        if (m_Guides.ContainsKey(index))
        {
            m_Guides[index].SetActive(flag);
        }
    }
}
