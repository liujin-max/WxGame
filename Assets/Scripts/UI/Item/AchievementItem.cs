using System.Collections;
using System.Collections.Generic;
using CB;
using TMPro;
using UnityEngine;


namespace CB
{
    public class AchievementItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_Description;
        [SerializeField] TextMeshProUGUI m_Reward;


        public void Init(Achievement achievement)
        {
            m_Description.text = achievement.GetDescription();
            m_Reward.text = achievement.GetRewardString();
        }
    }
}