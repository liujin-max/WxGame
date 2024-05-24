using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Money
{
    /// <summary>
    /// 日历 随着游戏时间而流逝
    /// </summary>
    public class Calendar
    {
        private CDTimer m_Timer = new CDTimer(10);
        public CDTimer Timer {get {return m_Timer;} }


        private int m_Year = 18;    //18岁出去辍学打工
        public int Year {get {return m_Year;}}

        private int m_Income = 0;   //年收入
        public int Income {get {return m_Income;}}

        public void Init()
        {

        }

        public void UpdateIncome(int value)
        {
            m_Income += value;
        }

        void Next()
        {
            m_Year++;
            m_Income = 0;

            EventManager.SendEvent(new GameEvent(EVENT.ONYEARUPDATE, m_Year));
        }

        public void Clock(float fixed_deltatime)
        {
            //每隔一段事件，调整物价
            m_Timer.Update(fixed_deltatime);
            if (m_Timer.IsFinished() == true) {
                m_Timer.Reset();

                Next();
            }
        }
    }
}