using System.Collections;
using System.Collections.Generic;
using Money;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Money
{
    public class SellWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private TextMeshProUGUI m_Count;
        [SerializeField] private TextMeshProUGUI m_Cost;
        [SerializeField] private Slider m_Slider;
        [SerializeField] private Button m_BtnConfirm;
        [SerializeField] private Button m_BtnCancel;


        private Package m_Package;
        private Goods m_Goods;
        private int m_Current = 1;


        void Awake()
        {
            Field.Instance.Pause();

            m_Slider.onValueChanged.AddListener((value)=>{
                m_Current = (int)value;
                FlushUI();
            });


            m_BtnConfirm.onClick.AddListener(()=>{
                Field.Instance.Market.SalePackages(m_Package.ID, m_Goods.Price, m_Current);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            m_BtnCancel.onClick.AddListener(()=>{
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        void OnDestroy()
        {
            Field.Instance.Resume();
        }

        public void Init(Package package)
        {
            m_Package   = package;
            m_Goods     = Field.Instance.Market.GetGoods(m_Package.ID);

            m_Text.text = package.Name;
            
            m_Slider.maxValue = package.Count;
            m_Slider.minValue = 1;

            FlushUI(); 
        }

        void FlushUI()
        {
            m_Count.text    = m_Current.ToString();
            m_Cost.text     = (m_Current * m_Goods.Price).ToString();
        }
    }
}

