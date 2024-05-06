using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CB
{
    public class AchievementItem : MonoBehaviour
    {
        private Achievement m_Achievement;

        [SerializeField] Button c_Touch;
        [SerializeField] TextMeshProUGUI c_Description;
        [SerializeField] TextMeshProUGUI c_Reward;
        [SerializeField] GameObject c_FinishMask;


        public void Init(Achievement achievement)
        {
            m_Achievement = achievement;

            c_Description.text = achievement.GetDescription();
            c_Reward.text = achievement.GetRewardString();

            FlushUI();
        }

        void FlushUI()
        {
            c_FinishMask.SetActive(m_Achievement.IsFinished);
        }

        public void OnClick()
        {
            m_Achievement.PopupWindow();
        }
    }
}