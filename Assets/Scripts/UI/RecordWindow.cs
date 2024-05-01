using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public void Init(int ach_coin, int ach_glass)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("当前最高记录: {0}层", GameFacade.Instance.User.Score));

        sb.Append(string.Format("\n\n获得<sprite=1>：{0}", GameFacade.Instance.Game.GetScoreCoin() + ach_coin));
        if (ach_coin > 0) {
            sb.Append(string.Format("({0}{1}</color> 成就奖励)", _C.GREENCOLOR2, ach_coin));
        } 

        sb.Append(string.Format("\n\n获得<sprite=0>：{0}", GameFacade.Instance.Game.GetScoreGlass() + ach_glass));
        if (ach_glass > 0) {
            sb.Append(string.Format("({0}{1}</color> 成就奖励)", _C.GREENCOLOR2, ach_glass));
        } 

        m_Description.GetComponent<ShakeText>().SetText(sb.ToString());
    }
}
