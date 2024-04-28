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

    private DataManager m_DataManager = null;
    public DataManager DataManager
    {
        get {
            if (m_DataManager == null) {
                m_DataManager = transform.AddComponent<DataManager>();
            }
            return m_DataManager;
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
        Application.targetFrameRate = 60;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(GameObject.Find("POOL"));
        DontDestroyOnLoad(GameObject.Find("Canvas"));
        DontDestroyOnLoad(GameObject.Find("RankCanvas"));
        DontDestroyOnLoad(GameObject.Find("Camera"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));



        #if WEIXINMINIGAME && !UNITY_EDITOR
            WX.InitSDK((code) =>
            {
                Init();
            });
        #else
            Init();
        #endif
    }

    void Init()
    {
        CsvManager.ReadCsvs();

        CONFIG.InitDatas();
        
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
