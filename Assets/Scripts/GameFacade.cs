using System.Collections;
using System.Collections.Generic;
using CB;
using Unity.VisualScripting;
using UnityEngine;
using WeChatWASM;

public class GameFacade : MonoBehaviour
{
    public bool Reboot = false;

    private TipWindow m_TipWindow;

    private User m_User;
    public User User
    {
        get {
            if (m_User == null) {
                m_User = new User();
            }
            return m_User;
        }
    }

    private DataCenter m_DataCenter = null;
    public DataCenter DataCenter
    {
        get {
            if (m_DataCenter == null) {
                m_DataCenter = new DataCenter();
            }
            return m_DataCenter;
        }
    }


    #region =====  Manager =====
    private OBJManager m_PoolManager = null;
    public OBJManager PoolManager
    {
        get {
            if (m_PoolManager == null) {
                m_PoolManager = transform.AddComponent<OBJManager>();
            }
            return m_PoolManager;
        }
    }

    private EventManager m_EventManager = null;
    public EventManager EventManager
    {
        get {
            if (m_EventManager == null) {
                m_EventManager = transform.AddComponent<EventManager>();
            }
            return m_EventManager;
        }
    }


    private ScenePool m_ScenePool = null;
    public ScenePool ScenePool
    {
        get {
            if (m_ScenePool == null) {
                m_ScenePool = transform.AddComponent<ScenePool>();
            }
            return m_ScenePool;
        }
    }

    private EffectManager m_EffectManager = null;
    public EffectManager EffectManager
    {
        get {
            if (m_EffectManager == null) {
                m_EffectManager = transform.AddComponent<EffectManager>();
            }
            return m_EffectManager;
        }
    }

    private SoundManager m_SoundManager = null;
    public SoundManager SoundManager
    {
        get {
            if (m_SoundManager == null) {
                m_SoundManager = transform.AddComponent<SoundManager>();
            }
            return m_SoundManager;
        }
    }

    private GameController m_GameController = null;
    public GameController Game
    {
        get {
            if (m_GameController == null) {
                m_GameController = GameObject.Find("Game").transform.GetComponent<GameController>();
            }
            return m_GameController;
        }
    }

    private UIManager m_UIManager = null;
    public UIManager UIManager
    {
        get {
            if (m_UIManager == null) {
                m_UIManager = transform.AddComponent<UIManager>();
            }
            return m_UIManager;
        }
    }

    private CsvManager m_CsvManager = null;
    public CsvManager CsvManager
    {
        get {
            if (m_CsvManager == null) {
                m_CsvManager = transform.AddComponent<CsvManager>();
            }
            return m_CsvManager;
        }
    }

    private SystemManager m_SystemManager = null;
    public SystemManager SystemManager
    {
        get {
            if (m_SystemManager == null) {
                m_SystemManager = transform.AddComponent<SystemManager>();
            }
            return m_SystemManager;
        }
    }

    #endregion

    private static GameFacade _instance = null;
    public static GameFacade Instance
    {
        get {return _instance;} 
    }

    void Awake()
    {
        _instance = this;

        Input.multiTouchEnabled = false;
        

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("POOL"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("RankCanvas"));
        DontDestroyOnLoad(GameObject.Find("Camera"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));

        
        #if WEIXINMINIGAME && !UNITY_EDITOR
            WX.InitSDK((code) =>
            {
                //初始化云开发
                CallFunctionInitParam param = new CallFunctionInitParam();
                param.env = _C.CLOUD_ENV;
                WX.cloud.Init(param);

                WX.SetPreferredFramesPerSecond(_C.DEFAULT_FRAME);
                WX.ReportGameStart();
                Init();
            });
        #else
            Application.targetFrameRate = _C.DEFAULT_FRAME;

            Init();
        #endif
    }

    void Init()
    {
        //初始化配置文件
        CsvManager.ReadCsvs();
        CONFIG.InitDatas();

        //加载账号数据
        User.Init();
        //加载数据类
        DataCenter.Init();
        
        m_TipWindow = UIManager.LoadWindow("Prefab/UI/TipWindow", UIManager.TIP).GetComponent<TipWindow>();

        NavigationController.GotoLoading();
    }

    public void FlyTip(string text)
    {
        if (m_TipWindow != null) {
            m_TipWindow.FlyTip(text);
        }
    }
}
