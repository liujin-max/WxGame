using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordWindow : MonoBehaviour 
{
    [SerializeField] private Transform c_Frame;

    [SerializeField] private TextMeshProUGUI c_Record;
    [SerializeField] private GameObject c_CoinPivot;
    [SerializeField] private TextMeshProUGUI c_Coin;
    [SerializeField] private GameObject c_GlassPivot;
    [SerializeField] private TextMeshProUGUI c_Glass;

    [SerializeField] private Button c_BtnReward;

    void Awake()
    {
        c_Frame.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        c_Frame.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
        c_Frame.DOScale(1, 0.15f);
    }

    // Start is called before the first frame update
    void Start()
    {
        c_BtnReward.onClick.AddListener(()=>{
            c_Frame.GetComponent<CanvasGroup>().DOFade(0, 0.15f);
            c_Frame.DOScale(1.5f, 0.15f).OnComplete(()=>{
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

        });
    }

    public void Init(int ach_coin, int ach_glass)
    {
        c_Record.text = string.Format("{0}层", GameFacade.Instance.User.Score);

        int coin_value = GameFacade.Instance.Game.GetScoreCoin() + ach_coin;
        if (coin_value > 0)
        {
            c_CoinPivot.SetActive(true);

            StringBuilder coin_sb = new StringBuilder();
            coin_sb.Append(string.Format("{0}{1}</color>", _C.GREENCOLOR, coin_value));
            if (ach_coin > 0) {
                coin_sb.Append(string.Format(" ( {0} 来自成就)", ach_coin));
            } 
            c_Coin.text = coin_sb.ToString();
        }
        else 
        {
            c_CoinPivot.SetActive(false);
        }


        int glass_value = GameFacade.Instance.Game.GetScoreGlass() + ach_glass;
        if (glass_value > 0)
        {
            c_GlassPivot.SetActive(true);

            StringBuilder glass_sb = new StringBuilder();
            glass_sb.Append(string.Format("{0}{1}</color>", _C.GREENCOLOR, glass_value));
            if (ach_glass > 0) {
                glass_sb.Append(string.Format(" ( {0} 来自成就)", ach_glass));
            } 
            c_Glass.text = glass_sb.ToString();
        }
        else 
        {
            c_GlassPivot.SetActive(false);
        }


        // m_Description.GetComponent<ShakeText>().SetText(sb.ToString());
    }
}
