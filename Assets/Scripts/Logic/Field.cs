using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{
    public class Field : MonoBehaviour
    {

        private int m_Coin;
        public int Coin { 
            get { return Crypt.DE(m_Coin); } 
        }

        


        private static Field m_Instance = null;
        public static Field Instance {get { return m_Instance; } }


        void Awake()
        {
            m_Instance = this;
        }


        // Update is called once per frame
        void Update()
        {
            
        }

        public void Enter()
        {
            m_Coin = Crypt.EN(0);


            GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM);
        }

        public void UpdateCoin(int value)
        {
            m_Coin = Crypt.EN(Coin + value);
        }
    }

}
