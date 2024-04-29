using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordWindow : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI m_Description;
    [SerializeField] private Button m_BtnReward;

    // Start is called before the first frame update
    void Start()
    {
        m_BtnReward.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    public void Init()
    {
        var sb = string.Format("当前最高记录: {0}层\n\n获得<sprite=1>：{1}\n\n获得<sprite=0>：{2}", GameFacade.Instance.User.Score, GameFacade.Instance.Game.GetScoreCoin(), GameFacade.Instance.Game.GetScoreGlass());

        m_Description.GetComponent<ShakeText>().SetText(sb);
    }
}
