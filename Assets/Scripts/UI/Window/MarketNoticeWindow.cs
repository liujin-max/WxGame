using System.Collections;
using System.Collections.Generic;
using System.Text;
using Money;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketNoticeWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Description;

    [SerializeField] private Button m_BtnConfirm;

    void Awake()
    {
        Field.Instance.Pause();

        m_BtnConfirm.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    void OnDestroy()
    {
        Field.Instance.Resume();
    }

    void Start()
    {
        InitDescription();
    }

    void InitDescription()
    {
        StringBuilder sb = new StringBuilder();

        Field.Instance.Market.Goods.ForEach(g => {
            if (g.IsUnsold()) {
                if (!string.IsNullOrEmpty(sb.ToString())) sb.Append("\n\n");

                sb.Append(g.ReductionDes);
            }

            if (g.IsHotSelling()) {
                if (!string.IsNullOrEmpty(sb.ToString())) sb.Append("\n\n");

                sb.Append(g.IncreaseDes);
            }
        });

        if (string.IsNullOrEmpty(sb.ToString())) {
            sb.Append("市场风平浪静...");
        }

        m_Description.text = sb.ToString();
    }
}
