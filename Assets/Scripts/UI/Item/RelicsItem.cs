using System.Collections;
using System.Collections.Generic;
using System.Text;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicsItem : MonoBehaviour
{
    public Relics m_Relics;
    public Button Touch;

    [SerializeField] GameObject c_Light;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private TextMeshProUGUI c_Cost;
    


    void Awake()
    {
        c_Light.SetActive(false);
    }
    
    public void Init(Relics data)
    {
        m_Relics    = data;

        this.FlushUI();
    }
    
    void FlushUI()
    {
        var relics  = m_Relics;

        c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
        c_Icon.SetNativeSize();

        var color = "<#FFFFFF>";
        if (relics.Price > GameFacade.Instance.Game.m_Coin) {
            color   = _C.REDCOLOR;
        }
        c_Cost.text = string.Format("( {0}{1} </color><sprite=1>)", color, relics.Price);
    }

    public void Select(bool flag)
    {
        c_Light.SetActive(flag);
        
        if (flag == true) {
            // 创建抖动和缩放效果
            Touch.transform.DOShakeScale(0.3f, 0.5f, vibrato: 15, randomness: 50, fadeOut: true).OnComplete(()=>{
                transform.localScale = Vector3.one;
            });
        } else {
            Touch.transform.DOScale(Vector3.one, 0.1f);
        }
    }

    public void ShowCost(bool flag)
    {
        c_Cost.gameObject.SetActive(flag);
    }
}
