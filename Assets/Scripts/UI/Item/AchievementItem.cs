using System.Collections;
using System.Collections.Generic;
using CB;
using DG.Tweening;
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

        private Animation m_Anim;
        
        void Awake()
        {
            c_Touch.onClick.AddListener(()=>{
                Debug.Log("onClick");
                m_Achievement.PopupWindow();
            });
        }

        public void Init(Achievement achievement)
        {
            m_Achievement = achievement;

            c_Description.text = achievement.GetDescription();
            c_Reward.text = achievement.GetRewardString();

            FlushUI();
        }

        public void FadeIn()
        {
            if (m_Anim == null) {
                m_Anim = transform.GetComponent<Animation>();
            }

            m_Anim.Play("ACHShow");
        }

        void FlushUI()
        {
            c_FinishMask.SetActive(m_Achievement.IsFinished);
        }
    }
}