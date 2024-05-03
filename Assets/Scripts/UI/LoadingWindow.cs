using System.Collections;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI c_Score;
        [SerializeField] private UrlImageUtility c_Head;
        [SerializeField] private Button BtnEnter;
        [SerializeField] private Button BtnRank;
        [SerializeField] private Button BtnAchievement;

        
        void Awake()
        {
            BtnEnter.onClick.AddListener(() => {
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                    GameFacade.Instance.Game.Enter();
                });

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnRank.onClick.AddListener(()=>{  
                Platform.Instance.PULLRANK();
            });

            BtnAchievement.onClick.AddListener(()=>{  
                var obj = GameFacade.Instance.UIManager.ShowWindow("AchievementWindow");
                if (obj == null) {
                    obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/AchievementWindow", GameFacade.Instance.UIManager.BOARD);
                    var window = obj.GetComponent<AchievementWindow>();
                    window.Init();
                } 
            });


            GameFacade.Instance.EventManager.AddHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        void OnDestroy()
        {
            GameFacade.Instance.EventManager.DelHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        // Start is called before the first frame update
        void Start()
        {
            FlushUI();
        }

        void FlushUI()
        {
            c_Head.SetImage(GameFacade.Instance.User.HeadURL);
            c_Score.text    = GameFacade.Instance.User.Score.ToString() + " 层";
        }

        void OnReponseFlushUser(GameEvent gameEvent)
        {
            FlushUI();
        }
    }
}
