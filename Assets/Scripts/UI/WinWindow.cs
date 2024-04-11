using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CB
{
    public class WinWindow : MonoBehaviour
    {
        [SerializeField] private Button BtnReturn;
        [SerializeField] private Button BtnNext;
        [SerializeField] private TextMeshProUGUI ChapterText;

        void Start()
        {
            BtnReturn.onClick.AddListener(()=>{
                NavigationController.GotoLoading();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnNext.onClick.AddListener(()=>{
                GameFacade.Instance.Game.Restart();
                GameFacade.Instance.Game.Enter();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        public void Init(int chapter)
        {
            ChapterText.text = string.Format("第 <#9bc042>{0}</color> 关", chapter + 1);
        }

    }
}

