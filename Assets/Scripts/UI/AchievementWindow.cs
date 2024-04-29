using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.UI;

public class AchievementWindow : MonoBehaviour
{
    [SerializeField] Button Mask;

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
        GameFacade.Instance.DataCenter.Achievements.ForEach(achievement => {
            // Debug.Log(achievement.Description);
        });
    }
}
