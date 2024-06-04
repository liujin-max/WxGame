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
    [SerializeField] private Button c_BtnCoin;

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
        Platform.Instance.BANNER_VIDEOAD("adunit-0a5910ddc759e7d3", true, 790);

        if (GameFacade.Instance.Game.Stage % 6 == 0)
            Platform.Instance.INTER_VIDEOAD("adunit-4b36fdd955b27425");

        c_BtnSkip.onClick.AddListener(()=> {
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            Platform.Instance.BANNER_VIDEOAD("adunit-0a5910ddc759e7d3", false);

            GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
        });

        c_BtnVideoRefresh.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            Platform.Instance.REWARD_VIDEOAD("adunit-1dc73ba6dff278d8", ()=>{
                List<Relics> datas = GameFacade.Instance.Game.GenerateRelicses();
                Init(datas);
            });
        });

        //看广告，获取金币
        c_BtnCoin.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            Platform.Instance.REWARD_VIDEOAD("adunit-708c6492458c7564", ()=>{
                c_BtnCoin.gameObject.SetActive(false);  //只能看1次
                GameFacade.Instance.Game.UpdateCoin(10, false);

                for (int i = 0; i < 5; i++) {
                    var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, c_BtnCoin.transform.position);
                    e.GetComponent<FlyCoin>().Fly(0.1f * i); 
                }
            }); 
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
