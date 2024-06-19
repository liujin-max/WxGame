using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class TipsPair
{
    public int Key;
    public GameObject Value;
}


public class LevelTipsWindow : MonoBehaviour
{
    [SerializeField] private GameObject m_Mask;
    [SerializeField] private List<TipsPair> m_Tips = new List<TipsPair>();


    void Awake()
    {
        EventManager.AddHandler(EVENT.ONENTERSTAGE,         OnReponseEnterStage);
    }

    void Start()
    {
        m_Mask.SetActive(false);

        m_Tips.ForEach(pair => {
            pair.Value.SetActive(false);
        });
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.ONENTERSTAGE,         OnReponseEnterStage);
    }

    #region 监听事件
    private void OnReponseEnterStage(GameEvent @event)
    {
        var stage_level = @event.GetParam(0) as Stage;


        foreach (var pair in m_Tips)
        {
            if (pair.Key == stage_level.ID)
            {
                m_Mask.SetActive(true);
                pair.Value.SetActive(true);

                pair.Value.transform.Find("BtnClose").GetComponent<Button>().onClick.AddListener(()=>{
                    m_Mask.SetActive(false);
                    pair.Value.SetActive(false);
                });

                break;
            }
            else if (pair.Key > stage_level.ID) {
                break;
            }
        }
    }
    #endregion
}
