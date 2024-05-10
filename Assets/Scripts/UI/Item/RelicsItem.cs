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
    [SerializeField] private GameObject c_Back;
    [SerializeField] private Transform c_Touch;
    [SerializeField] private TextMeshProUGUI c_Name;
    [SerializeField] private Image c_IconShadow;
    [SerializeField] private RawImage c_Icon;
    [SerializeField] private TextMeshProUGUI c_Description;
    [SerializeField] private TextMeshProUGUI c_Cost;
    
    public Button BtnBuy;


    void Awake()
    {
        EventManager.AddHandler(EVENT.UI_FLUSHSHOP,     OnReponseShop);
    }

    void OnDestroy()
    {
        EventManager.DelHandler(EVENT.UI_FLUSHSHOP,     OnReponseShop);
    }

    public void Init(Relics data)
    {
        Relics  = data;

        c_Shadow.SetActive(true);
        
        c_Touch.gameObject.SetActive(true);
        c_Touch.localEulerAngles = Vector3.zero;
        c_Touch.GetComponent<Shine>().Restart();

        c_Back.SetActive(false);
        c_Back.transform.localEulerAngles = Vector3.zero;

        this.FlushUI();
    }
    
    void FlushUI()
    {
        var relics  = Relics;

        c_Name.text = relics.Name;

        c_Icon.texture   = Resources.Load<Texture>("UI/Relics/" + relics.ID);
        c_Icon.SetNativeSize();

        c_IconShadow.sprite = Resources.Load<Sprite>("UI/Relics/" + relics.ID);
        c_IconShadow.SetNativeSize();

        c_Description.text = relics.GetDescription();

        var color = "<#FFFFFF>";
        if (relics.Price > GameFacade.Instance.Game.Coin) {
            color   = _C.REDCOLOR;
        }
        c_Cost.text = string.Format(" {0}<size=46>{1}</size></color> <sprite=1>", color, relics.Price);
    }

    public void TurnBack()
    {
        StartCoroutine("FadeBack");
    }

    //翻面
    IEnumerator FadeBack()
    {
        c_Touch.localEulerAngles = Vector3.zero;

        c_Back.SetActive(false);
        c_Shadow.SetActive(false);

        float time = 0.13f;
        c_Touch.DOLocalRotate(new Vector3(0, 90, 0), time, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(time);
        c_Touch.gameObject.SetActive(false);

        c_Back.SetActive(true);
        c_Back.transform.localEulerAngles = new Vector3(0, 90, 0);
        c_Back.transform.DOLocalRotate(new Vector3(0, 0, 0), time, RotateMode.FastBeyond360);


        yield return null;
    }

    private void OnReponseShop(GameEvent @event)
    {
        this.FlushUI();
    }
}
