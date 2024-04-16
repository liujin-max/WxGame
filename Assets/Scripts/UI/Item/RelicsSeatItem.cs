using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RelicsSeatItem : MonoBehaviour
{
    public Relics m_Relics;

    private Sequence m_Sequence;

    [SerializeField] private Image c_Icon;
    [SerializeField] private TextMeshProUGUI c_Value;

    void Awake()
    {
        GameFacade.Instance.EventManager.AddHandler(EVENT.UI_TRIGGERRELICS,    OnRelicsTrigger);
    }

    void OnDestroy()
    {
        GameFacade.Instance.EventManager.DelHandler(EVENT.UI_TRIGGERRELICS,    OnRelicsTrigger);
    }

    public void Init(Relics relics = null)
    {
        m_Relics = relics;

        if (relics == null) {
            c_Icon.gameObject.SetActive(false);
            c_Value.text = "";

        } else {
            c_Icon.gameObject.SetActive(true);


            c_Icon.sprite = Resources.Load<Sprite>("UI/Relics/" + relics.ID);
            c_Icon.SetNativeSize();

            c_Value.text = relics.ShowValue();
        }
    }

    void Update()
    {
        if (m_Relics != null) {
            c_Value.text = m_Relics.ShowValue();
        }
    }


    void ShowScale()
    {
        GameFacade.Instance.SoundManager.Load(SOUND.TRIGGER);
        
        GameObject obj = new GameObject("Image");
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        Image imageComponent = obj.AddComponent<Image>();
        imageComponent.sprite = c_Icon.sprite;
        imageComponent.SetNativeSize();
        
        Sequence seq = DOTween.Sequence();
        seq.Join(imageComponent.transform.DOScale(1.5f, 0.4f));
        seq.Join(imageComponent.DOFade(0f, 0.4f).OnComplete(()=>{
            Destroy(obj);
        }));

        seq.Play();

    }

    void OnRelicsTrigger(GameEvent gameEvent)
    {
        var relics = (Relics)gameEvent.GetParam(0);
        if (relics != m_Relics) return;

        ShowScale();
    }
}
