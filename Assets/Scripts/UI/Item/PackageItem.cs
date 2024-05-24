using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Money
{
    public class PackageItem : MonoBehaviour
    {
        private Package m_Package;

        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Cost;
        [SerializeField] private TextMeshProUGUI m_Count;

        void Awake()
        {
            transform.GetComponent<Button>().onClick.AddListener(()=>{
                var window = GameFacade.Instance.UIManager.LoadWindow("SellWindow", UIManager.BOARD).GetComponent<SellWindow>();
                window.Init(m_Package);
            });
        }
        
        public void Init(Package pkg)
        {
            m_Package = pkg;

            m_Title.text = pkg.Name;

            FlushUI();
        }

        void FlushUI()
        {
            m_Cost.text = m_Package.Price.ToString();
            m_Count.text = m_Package.Count.ToString();
        }
    }
}
