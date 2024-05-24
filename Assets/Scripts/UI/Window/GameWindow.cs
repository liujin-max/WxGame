using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Money
{
   public class GameWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Coin;
        [SerializeField] private Button m_BtnCoin;
        [SerializeField] private Image m_TimerBar;

        [SerializeField] private Transform m_Pivot;
        


        private MarketItem m_MarketItem = null;


        void Awake()
        {
            m_TimerBar.fillAmount = 0;

            m_BtnCoin.GetComponent<LongPressButton>().Init(()=>{
                Field.Instance.UpdateCoin(1);

                var e = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.FLYCOIN, m_BtnCoin.transform.position);
                e.GetComponent<FlyCoin>().Fly();
            });
        }


        // Update is called once per frame
        void Update()
        {
            m_TimerBar.fillAmount = Field.Instance.Market.Timer.Current / Field.Instance.Market.Timer.Duration;

            m_Coin.text = Field.Instance.Coin.ToString();  
        }

        void HideItems()
        {
            if (m_MarketItem != null) m_MarketItem.gameObject.SetActive(false);


        }

        public void ShowMarket()
        {
            HideItems();

            if (m_MarketItem == null) {
                m_MarketItem = GameFacade.Instance.UIManager.LoadItem("MarketItem", m_Pivot).GetComponent<MarketItem>();
                m_MarketItem.InitGoods();
            }
            m_MarketItem.gameObject.SetActive(true);
        }
    }
 
}
