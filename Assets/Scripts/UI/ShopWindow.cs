using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindow : MonoBehaviour
{
    [SerializeField] Transform c_Pivot;
    [SerializeField] Button c_BtnSkip;
    [SerializeField] Button c_BtnVideoRefresh;


    private List<Relics> m_Relicses;
    private List<RelicsItem> m_Items = new List<RelicsItem>();


    RelicsItem new_relics_item(int order)
    {
        RelicsItem item = null;
        if (m_Items.Count > order){
            item = m_Items[order];
        } else {
            GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/RelicsItem", c_Pivot);
            item = obj.GetComponent<RelicsItem>();

            m_Items.Add(item);
        }

        item.gameObject.SetActive(true);

        return item;
    }

    void Start()
    {
        c_BtnSkip.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
        });

        c_BtnVideoRefresh.gameObject.SetActive(false);
        c_BtnVideoRefresh.onClick.AddListener(()=>{
            // List<Relics> datas = GameFacade.Instance.Game.GenerateRelicses();
            
            // Init(datas);
        });

    }

    public void Init(List<Relics> relicses)
    {
        m_Relicses = relicses;

        InitRelicses(relicses);
    }

    public void InitRelicses(List<Relics> relicses)
    {
        foreach (var item in m_Items) {
            item.gameObject.SetActive(false);
        }


        for (int i = 0; i < relicses.Count; i++)
        {
            var relics      = relicses[i];
            RelicsItem item = new_relics_item(i);
            item.transform.localPosition = new Vector3((i - ((relicses.Count - 1) / 2.0f)) * 350, 0, 0); 
            item.Init(relics);

            item.BtnBuy.onClick.RemoveAllListeners();
            item.BtnBuy.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

                if (GameFacade.Instance.Game.BuyRelics(item.Relics) != null) {
                    GameFacade.Instance.SoundManager.Load(SOUND.COST);
                    item.BtnBuy.onClick.RemoveAllListeners();   //防止重复购买

                    item.TurnBack();
                }
            });
        }
    }
}
