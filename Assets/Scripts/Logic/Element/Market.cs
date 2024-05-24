using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{

    /// <summary>
    /// 市场：负责生成商品和存放我当前仓库的商品
    /// </summary>
    public class Market 
    {
        private List<Goods> m_Goods = new List<Goods>();
        public List<Goods> Goods { get { return m_Goods; } }

        private List<Package> m_Packages = new List<Package>();
        private Dictionary<int, Package> m_PackageDic = new Dictionary<int, Package>();
        public List<Package> Packages { get { return m_Packages; } }

        public void Init()
        {
            InitGoods();
        }

        //生成商品
        void InitGoods()
        {
            m_Goods.Clear();

            GameFacade.Instance.DataCenter.GetGoodsDatas().ForEach(data => {
                Goods goods = new Goods(data);
                m_Goods.Add(goods);
            });
        }

        public Package GetPackage(int id)
        {
            if (m_PackageDic.ContainsKey(id))
            {
                return m_PackageDic[id];
            }
            return null;
        }

        //购买商品
        public void BuyGoods(int id, int price, int count)
        {
            //判断价格够不够
            int cost = price * count;
            if (!Field.Instance.CheckCoinEnough(cost)) {
                return;
            }


            Package pkg = this.GetPackage(id);
            if (pkg == null) {
                var data = GameFacade.Instance.DataCenter.GetGoods(id);
                pkg = new Package(data);

                m_Packages.Add(pkg);
                m_PackageDic[id] = pkg;
            }

            pkg.Increase(price, count);
            Field.Instance.UpdateCoin(-cost);

            //通知UI
            EventManager.SendEvent(new GameEvent(EVENT.UI_BUYGOODS, pkg));
        }

        //出售货物
        public void SellPackages(int id, int price, int count)
        {
            Package pkg = this.GetPackage(id);
            if (pkg == null) {
                return;
            }

            count = Mathf.Min(count, pkg.Count);
            int reward = price * count;

            pkg.Decrease(price, count);
            Field.Instance.UpdateCoin(reward);

            if (pkg.Count <= 0) {
                m_Packages.Remove(pkg);
                m_PackageDic.Remove(id);
            }

            //通知UI
            EventManager.SendEvent(new GameEvent(EVENT.UI_SELLPACKAGES, pkg));
        }
    }
}
