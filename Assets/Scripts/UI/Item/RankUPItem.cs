using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUPItem : MonoBehaviour
{
    private RankData m_RankData;


    [SerializeField] private Transform c_RotatePivot;
    [SerializeField] private Transform c_TargetPivot;
    [SerializeField] private Transform c_MyPivot;
    [SerializeField] private Text c_Title;
    [SerializeField] private Text c_Order;
    [SerializeField] private GameObject c_Arrow;
    [SerializeField] private Text c_Surpass;

    public void Init(RankData rankData)
    {
        m_RankData = rankData;

        InitTarget(rankData);
        InitHead();


        StartCoroutine("UPAnim");
    }

    IEnumerator UPAnim()
    {
        c_Title.text = "";
        c_Order.text = "";
        c_Arrow.SetActive(false);
        c_Surpass.gameObject.SetActive(false);

        transform.DOLocalMoveX(100, 0.3f);

        yield return new WaitForSeconds(0.4f);

        float time = 0.7f;
        c_RotatePivot.DOLocalRotate(new Vector3(0, 0, -180), time, RotateMode.FastBeyond360);
        c_TargetPivot.DOScale(0.4f, time);
        c_MyPivot.DOScale(0.85f, time);

        yield return new WaitForSeconds(0.4f);

        float s_time = 0.2f;
        c_Surpass.gameObject.SetActive(true);
        c_Surpass.transform.localScale = new Vector3(5, 5, 5);
        c_Surpass.color = new Color(233 / 255f, 43 / 255f, 43 / 255f, 0);

        c_Surpass.transform.DOScale(2, s_time);
        c_Surpass.DOFade(1, s_time);

        yield return new WaitForSeconds(s_time);
        yield return new WaitForSeconds(0.1f);
        c_TargetPivot.GetComponent<CanvasGroup>().DOFade(0, 1f);
        c_Surpass.DOFade(0, 1f);

        c_Title.DOText("排名提升！！！", 0.6f);
        yield return new WaitForSeconds(0.6f);

        c_Order.DOText(string.Format("当前：<color=#15CF00>{0}</color>", m_RankData.Order), 0.6f);
        yield return new WaitForSeconds(0.4f);

        c_Arrow.SetActive(true);
        c_Arrow.transform.DOMoveY(c_Arrow.transform.position.y + 0.1f, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

        yield return new WaitForSeconds(1.5f);

        Destroy(gameObject);
    }

    void InitTarget(RankData rankData)
    {
        var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", c_TargetPivot).GetComponent<HeadItem>();
        item.Init(rankData.Head); 
    }

    void InitHead()
    {
        var item = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", c_MyPivot).GetComponent<HeadItem>();
        item.Init(GameFacade.Instance.User.HeadURL); 
    }

    void FixedUpdate()
    {
        c_TargetPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        c_MyPivot.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    
}
