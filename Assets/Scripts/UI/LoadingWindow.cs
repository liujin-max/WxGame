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
        [SerializeField] private Transform c_HeadPivot;
        [SerializeField] private TextMeshProUGUI c_Score;
        [SerializeField] private Button BtnEnter;
        [SerializeField] private Button BtnRank;
        [SerializeField] private Button BtnAchievement;


        private HeadItem m_HeadItem = null;

        
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


            EventManager.AddHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        void OnDestroy()
        {
            EventManager.DelHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        // Start is called before the first frame update
        void Start()
        {
            FlushUI();
        }

        void InitHead(string head_url)
        {
            if (m_HeadItem == null) {
                m_HeadItem = GameFacade.Instance.UIManager.LoadItem("Prefab/UI/Item/HeadItem", c_HeadPivot).GetComponent<HeadItem>();
            } 
            m_HeadItem.Init(head_url);
        }

        void FlushUI()
        {
            InitHead(GameFacade.Instance.User.HeadURL);
            
            c_Score.text    = GameFacade.Instance.User.Score.ToString() + " 层";
        }

        void OnReponseFlushUser(GameEvent gameEvent)
        {
            FlushUI();
        }
    }
}
