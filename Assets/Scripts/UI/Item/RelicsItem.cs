using System;
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

    [SerializeField] private GameObject c_Shadow;
    [SerializeField] private Transform c_Touch;
    [SerializeField] private TextMeshProUGUI c_Name;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private TextMeshProUGUI c_Description;
    [SerializeField] private TextMeshProUGUI c_Cost;
    
    public Button BtnBuy;

    public void Init(Relics data)
    {
        Relics  = data;

        c_Shadow.SetActive(true);
        c_Touch.localPosition = Vector3.zero;

        this.FlushUI();
    }
    
    void FlushUI()
    {
        var relics  = Relics;

        c_Name.text = relics.Name;

        c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
        c_Icon.SetNativeSize();

        c_Description.text = relics.GetDescription();

        var color = "<#FFFFFF>";
        if (relics.Price > GameFacade.Instance.Game.Coin) {
            color   = _C.REDCOLOR;
        }
        c_Cost.text = string.Format(" {0}<size=46>{1}</size></color> <sprite=1>", color, relics.Price);
    }

    public void MoveOut(Action callback)
    {
        c_Shadow.SetActive(false);

        c_Touch.DOLocalMoveY(800, 0.15f).OnComplete(()=>{
            callback();
        });
    }
}
