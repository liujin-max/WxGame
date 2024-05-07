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

        [SerializeField] private SuperScrollView c_ScrollView;
 

        // Start is called before the first frame update
        void Start()
        {
            Mask.onClick.AddListener(()=>{
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        public void Init()
        {
            InitAchievements();
        }

        void InitAchievements()
        {
            var list    = new List<Achievement>();
            GameFacade.Instance.DataCenter.Achievements.ForEach(ach => {
                if (ach.IsShow) {
                    // list.Add(ach);
                }
            });
            // list.Sort((a1, a2) => a1.SortOrder.CompareTo(a2.SortOrder));

            c_ScrollView.Init(list.Count, (obj, index, is_init)=>{
                AchievementItem item = obj.transform.GetComponent<AchievementItem>();
                item.Init(list[index]);
                if (!is_init) item.FadeIn();
            });
        }
    }
}

