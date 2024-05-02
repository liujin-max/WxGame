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
    public Relics Relics;

    [SerializeField] private TextMeshProUGUI c_Name;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private ShakeText c_Description;
    [SerializeField] private TextMeshProUGUI c_Cost;
    
    public Button BtnBuy;
    
    public void Init(Relics data)
    {
        Relics  = data;

        this.FlushUI();
    }
    
    void FlushUI()
    {
        var relics  = Relics;

        c_Name.text = relics.Name;

        c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
        c_Icon.SetNativeSize();

        c_Description.SetText(relics.GetDescription());

        var color = "<#FFFFFF>";
        if (relics.Price > GameFacade.Instance.Game.m_Coin) {
            color   = _C.REDCOLOR;
        }
        c_Cost.text = string.Format("{0}{1}</color><sprite=1>", color, relics.Price);
    }
}
