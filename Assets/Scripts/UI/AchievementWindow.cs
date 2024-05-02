using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.UI;


namespace CB
{
    public class AchievementWindow : MonoBehaviour
    {
        [SerializeField] Button Mask;
        [SerializeField] Transform c_Content;


        private List<AchievementItem> m_Items = new List<AchievementItem>();


        AchievementItem new_item(int order)
        {
            AchievementItem item = null;
            if (m_Items.Count > order){
                item = m_Items[order];
            } else {
                GameObject obj = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/AchievementItem", c_Content);
                item = obj.GetComponent<AchievementItem>();

                m_Items.Add(item);
            }
            item.gameObject.SetActive(true);
            return item;
        }


        // Start is called before the first frame update
        void Start()
        {
            Mask.onClick.AddListener(()=>{
                GameFacade.Instance.UIManager.HideWindow("AchievementWindow", gameObject);
            });
        }

        public void Init()
        {
            InitAchievements();
        }

        void InitAchievements()
        {
            m_Items.ForEach(item => {
                item.gameObject.SetActive(false);
            });

            int count = 0;
            GameFacade.Instance.DataCenter.Achievements.ForEach(ach => {
                if (ach.IsShow) {
                    var item = new_item(count);
                    item.Init(ach);
                    count++;
                }
            });
        }
    }
}

