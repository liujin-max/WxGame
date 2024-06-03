using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.UI;

public class GuideWindow : MonoBehaviour
{
    [SerializeField] Button c_GuideCoin;
    [SerializeField] Button c_GuideScore;
    [SerializeField] Button c_GuideBall;
    [SerializeField] Button c_GuideSwitch;

    private HashSet<Button> m_Guides = new HashSet<Button>();


    void Awake()
    {
        c_GuideCoin.gameObject.SetActive(false);
        c_GuideScore.gameObject.SetActive(false);
        c_GuideBall.gameObject.SetActive(false);
        c_GuideSwitch.gameObject.SetActive(false);

        c_GuideCoin.onClick.AddListener(()=>{
            c_GuideCoin.gameObject.SetActive(false);
            m_Guides.Add(c_GuideCoin);
        });

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
        if (m_Guides.Contains(c_GuideBall) == false)
        {
            c_GuideBall.gameObject.SetActive(true);

            //适配遮罩高度
            var seat_pivot = GameFacade.Instance.Game.GameUI.SeatPivot;
            var pos = new Vector3(seat_pivot.transform.position.x * 100, seat_pivot.transform.position.y * 100 + 5, 0);
            c_GuideBall.transform.GetComponent<UIMaskUtility>().SetCenter(pos);

            return;
        }

        if (m_Guides.Contains(c_GuideSwitch) == false)
        {
            c_GuideSwitch.gameObject.SetActive(true);
            
            var seat_pivot = GameFacade.Instance.Game.GameUI.SeatPivot;
            var pos = new Vector3(seat_pivot.transform.position.x * 100, seat_pivot.transform.position.y * 100 + 5, 0);
            c_GuideSwitch.transform.GetComponent<UIMaskUtility>().SetCenter(pos);

            return;
        }

        if (m_Guides.Contains(c_GuideScore) == false)
        {
            c_GuideScore.gameObject.SetActive(true);
            return;
        }

        if (m_Guides.Contains(c_GuideCoin) == false)
        {
            c_GuideCoin.gameObject.SetActive(true);

            //适配遮罩高度
            var seat_pivot = GameFacade.Instance.Game.GameUI.BarPivot;
            var pos = new Vector3(seat_pivot.transform.position.x * 100, seat_pivot.transform.position.y * 100 + 50, 0);
            c_GuideCoin.transform.GetComponent<UIMaskUtility>().SetCenter(pos);
            
            return;
        }

        GameFacade.Instance.SystemManager.SetIntByKey(SystemManager.KEY_GUIDE, 1);
        GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_PLAY);
        Destroy(gameObject);
    }
}
