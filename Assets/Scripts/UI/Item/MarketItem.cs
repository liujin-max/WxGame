using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{
    public class MarketItem : MonoBehaviour
    {
        [SerializeField] private Transform m_GoodsContent;
        [SerializeField] private Transform m_PackageContent;

    

        private List<GoodsItem> m_GoodsItems = new List<GoodsItem>();
        private List<PackageItem> m_PkgItems  = new List<PackageItem>();

        GoodsItem new_goods_item(int order)
        {
            GoodsItem item = null;

            if (order < m_GoodsItems.Count)  {
                item = m_GoodsItems[order];
            } else {
                item = GameFacade.Instance.UIManager.LoadItem("GoodsItem", m_GoodsContent).GetComponent<GoodsItem>();
                m_GoodsItems.Add(item);
            }

            item.gameObject.SetActive(true);

            return item;
        }

        PackageItem new_pkg_item(int order)
        {
            PackageItem item = null;

            if (order < m_PkgItems.Count)  {
                item = m_PkgItems[order];
            } else {
                item = GameFacade.Instance.UIManager.LoadItem("PackageItem", m_PackageContent).GetComponent<PackageItem>();
                m_PkgItems.Add(item);
            }

            item.gameObject.SetActive(true);

            return item;
        }

        void Awake()
        {
            EventManager.AddHandler(EVENT.UI_BUYGOODS,      OnReponseBuyGoods);
            EventManager.AddHandler(EVENT.UI_SELLPACKAGES,  OnReponseSellPkgs);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.UI_BUYGOODS,      OnReponseBuyGoods);
            EventManager.DelHandler(EVENT.UI_SELLPACKAGES,  OnReponseSellPkgs);
        }   


        public void InitGoods()
        {
            for (int i = 0; i < Field.Instance.Market.Goods.Count; i++)
            {
                var g = Field.Instance.Market.Goods[i];
                var item = new_goods_item(i);
                item.Init(g);
            }

        }


        void FlushPackages()
        {
            m_PkgItems.ForEach(item => item.gameObject.SetActive(false));

            for (int i = 0; i < Field.Instance.Market.Packages.Count; i++)
            {
                var g = Field.Instance.Market.Packages[i];
                var item = new_pkg_item(i);
                item.Init(g);
            }
        }


        #region 监听事件
        private void OnReponseSellPkgs(GameEvent @event)
        {
            FlushPackages();
        }

        private void OnReponseBuyGoods(GameEvent @event)
        {
            FlushPackages();
        }
        #endregion
    }
}
