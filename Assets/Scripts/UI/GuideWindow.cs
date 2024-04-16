using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.UI;

public class GuideWindow : MonoBehaviour
{
    [SerializeField] Button c_GuideScore;
    [SerializeField] Button c_GuideBall;
    [SerializeField] Button c_GuideSwitch;

    private HashSet<Button> m_Guides = new HashSet<Button>();


    void Awake()
    {
        c_GuideScore.gameObject.SetActive(false);
        c_GuideBall.gameObject.SetActive(false);
        c_GuideSwitch.gameObject.SetActive(false);



        c_GuideScore.onClick.AddListener(()=>{
            c_GuideScore.gameObject.SetActive(false);
            m_Guides.Add(c_GuideScore);
        });

        c_GuideBall.onClick.AddListener(()=>{
            c_GuideBall.gameObject.SetActive(false);
            m_Guides.Add(c_GuideBall);
        });

        c_GuideSwitch.onClick.AddListener(()=>{
            c_GuideSwitch.gameObject.SetActive(false);
            m_Guides.Add(c_GuideSwitch);
        });
    }

    void Update()
    {
        if (m_Guides.Contains(c_GuideScore) == false)
        {
            c_GuideScore.gameObject.SetActive(true);
            return;
        }

        if (m_Guides.Contains(c_GuideBall) == false)
        {
            c_GuideBall.gameObject.SetActive(true);
            return;
        }

        if (m_Guides.Contains(c_GuideSwitch) == false)
        {
            c_GuideSwitch.gameObject.SetActive(true);
            return;
        }


        GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_PLAY);
        GameFacade.Instance.DataManager.SetIntByKey(DataManager.KEY_GUIDE, 1);  //存储记录
        Destroy(gameObject);
    }
}
