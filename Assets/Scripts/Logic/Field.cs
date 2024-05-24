using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{
    public class Field : MonoBehaviour
    {

        private int m_Coin;
        public int Coin { get { return m_Coin; }}

        public Calendar Calendar;
        public Market Market;


        private static Field m_Instance = null;
        public static Field Instance {get { return m_Instance; } }


        void Awake()
        {
            m_Instance = this;
        }

        public void Enter()
        {
            m_Coin = 0;

            Calendar = new Calendar();
            Calendar.Init();

            Market = new Market();
            Market.Init();



            var window = GameFacade.Instance.UIManager.LoadWindow("GameWindow", UIManager.BOTTOM).GetComponent<GameWindow>();
            window.ShowMarket();
        }

        public void UpdateCoin(int value)
        {
            m_Coin += value;

            if (value > 0) Calendar.UpdateIncome(value);
        }

        public bool CheckCoinEnough(int cost)
        {
            return m_Coin >= cost;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
            float fixed_deltatime = Time.fixedDeltaTime;

            if (Calendar != null) {
                Calendar.Clock(fixed_deltatime);
            }
        }
    }

}
