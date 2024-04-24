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
    [SerializeField] Button c_BtnVideoRefresh;

    [SerializeField] GameObject c_DescriptionPivot;



    private List<Relics> m_Relicses;
    private RelicsItem m_SelectItem = null;
    private List<RelicsItem> m_Items = new List<RelicsItem>();


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
                // GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);

                m_Relicses.Remove(m_SelectItem.m_Relics);
                this.Init(m_Relicses);
            }
        });


        c_BtnCancel.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
        });

        c_BtnVideoRefresh.onClick.AddListener(()=>{
            List<Relics> datas = GameFacade.Instance.Game.GenerateRelicses();
            
            Init(datas);
        });

    }

    public void Init(List<Relics> relicses)
    {
        m_Relicses = relicses;

        InitRelicses(relicses);
    }

    public void InitRelicses(List<Relics> relicses)
    {
        c_DescriptionPivot.SetActive(false);
        c_BtnSelect.gameObject.SetActive(false);

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
