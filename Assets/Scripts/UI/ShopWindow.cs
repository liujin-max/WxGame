using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindow : MonoBehaviour
{
    [SerializeField] Transform c_OurPivot;
    [SerializeField] Transform c_SellPivot;
    [SerializeField] Button c_BtnSelect;
    [SerializeField] Button c_BtnCancel;
    [SerializeField] Button c_BtnSell;

    [SerializeField] GameObject c_DescriptionPivot;



    private List<Relics> m_Relicses;
    private RelicsItem m_SelectItem = null;

    private List<RelicsItem> m_OurItems = new List<RelicsItem>();
    private List<RelicsItem> m_Items = new List<RelicsItem>();


    RelicsItem new_our_item(int order)
    {
        RelicsItem item = null;
        if (m_OurItems.Count > order){
            item = m_OurItems[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RelicsItem", c_OurPivot);
            item = obj.GetComponent<RelicsItem>();

            m_OurItems.Add(item);
        }

        item.gameObject.SetActive(true);
        item.ShowCost(false);

        return item;
    }

    RelicsItem new_relics_item(int order)
    {
        RelicsItem item = null;
        if (m_Items.Count > order){
            item = m_Items[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RelicsItem", c_SellPivot);
            item = obj.GetComponent<RelicsItem>();

            m_Items.Add(item);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    void Start()
    {
        c_DescriptionPivot.SetActive(false);
        c_BtnSelect.gameObject.SetActive(false);

        c_BtnSelect.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            if (GameFacade.Instance.Game.BuyRelics(m_SelectItem.m_Relics) != null)
            {
                GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
            }
        });

        c_BtnSell.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.Army.RemoveRelics(m_SelectItem.m_Relics);

            InitOurs();
        });


        c_BtnCancel.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
        });

    }

    public void Init(List<Relics> relicses)
    {
        m_Relicses = relicses;

        // InitOurs();
        InitRelicses(relicses);
    }



    public void InitOurs()
    {
        c_BtnSelect.gameObject.SetActive(false);
        c_BtnSell.gameObject.SetActive(false);

        if (m_SelectItem != null) {
            m_SelectItem.Select(false);
        }
        m_SelectItem = null;

        foreach (var item in m_OurItems) {
            item.gameObject.SetActive(false);
        }

        var relicses = GameFacade.Instance.Game.Army.GetRelicses();
        for (int i = 0; i < relicses.Count; i++)
        {
            var relics      = relicses[i];
            RelicsItem item = new_our_item(i);
            item.transform.localPosition = new Vector3((i - ((relicses.Count - 1) / 2.0f)) * 290, 0, 0); 
            item.Init(relics);

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.BUBBLE);
                
                if (m_SelectItem != null) {
                    m_SelectItem.Select(false);
                }

                m_SelectItem = item;
                m_SelectItem.Select(true);

                c_BtnSelect.gameObject.SetActive(false);
                c_BtnSell.gameObject.SetActive(true);

                this.ShowDescription(relics);
            });
        }
    }

    public void InitRelicses(List<Relics> relicses)
    {
        c_BtnSelect.gameObject.SetActive(false);
        c_BtnSell.gameObject.SetActive(false);

        if (m_SelectItem != null) {
            m_SelectItem.Select(false);
        }
        m_SelectItem = null;

        foreach (var item in m_Items) {
            item.gameObject.SetActive(false);
        }


        for (int i = 0; i < relicses.Count; i++)
        {
            var relics      = relicses[i];
            RelicsItem item = new_relics_item(i);
            item.transform.localPosition = new Vector3((i - ((relicses.Count - 1) / 2.0f)) * 290, 0, 0); 
            item.Init(relics);

            item.Touch.onClick.RemoveAllListeners();
            item.Touch.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.BUBBLE);
                
                if (m_SelectItem != null) {
                    m_SelectItem.Select(false);
                }

                m_SelectItem = item;
                m_SelectItem.Select(true);

                c_BtnSelect.gameObject.SetActive(true);
                c_BtnSell.gameObject.SetActive(false);

                this.ShowDescription(relics);
            });
        }
    }

    void ShowDescription(Relics relics)
    {
        c_DescriptionPivot.SetActive(true);

        c_DescriptionPivot.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = relics.Name;
        c_DescriptionPivot.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = relics.GetDescription();
    }
}
