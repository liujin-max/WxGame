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


        void Awake()
        {
            m_BtnCoin.onClick.AddListener(()=>{
                Field.Instance.UpdateCoin(1);
            });
        }


        // Update is called once per frame
        void Update()
        {
            m_Coin.text = Field.Instance.Coin.ToString();
        }
    }
 
}
