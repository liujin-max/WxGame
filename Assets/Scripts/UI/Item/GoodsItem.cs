using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Money
{
    public class GoodsItem : MonoBehaviour
    {
        private Goods m_Goods;

        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Price;

        void Awake()
        {
            transform.GetComponent<Button>().onClick.AddListener(()=>{
                //一个都买不起
                if (!Field.Instance.CheckCoinEnough(m_Goods.Price)){
                    return;
                }

                var window = GameFacade.Instance.UIManager.LoadWindow("BuyWindow", UIManager.BOARD).GetComponent<BuyWindow>();
                window.Init(m_Goods);
            });
        }

        public void Init(Goods goods)
        {
            m_Goods = goods;

            m_Title.text = goods.Name;

            FlushUI();
        }

        void FlushUI()
        {
            m_Price.text = m_Goods.Price.ToString();
        }
    }
}
