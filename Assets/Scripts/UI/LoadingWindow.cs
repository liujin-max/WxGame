using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



namespace CB
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private Transform c_HeadPivot;
        [SerializeField] private TextMeshProUGUI c_Score;
        [SerializeField] private Button BtnEnter;
        [SerializeField] private Button BtnContinue;
        [SerializeField] private Button BtnSet;
        [SerializeField] private Button BtnRank;
        [SerializeField] private Button BtnAchievement;
        [SerializeField] private TextMeshProUGUI c_Version;


        private HeadItem m_HeadItem = null;

        
        void Awake()
        {
            Platform.Instance.GRID_VIDEOAD("adunit-8c9a560ad3540b0e", true);
            c_Version.text = "v" + GameFacade.Instance.Version;

            BtnEnter.onClick.AddListener(() => {
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                    GameFacade.Instance.Game.Enter();
                });

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnContinue.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                    GameFacade.Instance.Game.Enter(GameFacade.Instance.User.GetArchiveRecord());
                });

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnSet.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/SettingWindow", GameFacade.Instance.UIManager.BOARD);
            });

            BtnRank.onClick.AddListener(()=>{  
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                Platform.Instance.PULLRANK();
            });

            BtnAchievement.onClick.AddListener(()=>{  
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

                var obj = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/AchievementWindow", GameFacade.Instance.UIManager.BOARD);
                var window = obj.GetComponent<AchievementWindow>();
                window.Init();
            });


            EventManager.AddHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        void OnDestroy()
        {
            Platform.Instance.GRID_VIDEOAD("adunit-8c9a560ad3540b0e", false);

            EventManager.DelHandler(EVENT.UI_FLUSHUSER, OnReponseFlushUser);
        }

        // Start is called before the first frame update
        void Start()
        {
            ArchiveRecord archiveRecord = GameFacade.Instance.User.GetArchiveRecord();
            
            BtnEnter.gameObject.SetActive(archiveRecord == null);
            BtnContinue.gameObject.SetActive(archiveRecord != null);

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
