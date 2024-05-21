using System;
using System.Collections;
using System.Collections.Generic;
using CB;
using Unity.VisualScripting;
using UnityEngine;


public class GameFacade : MonoBehaviour
{

    public string Version = "1.0.5";

    private TipWindow m_TipWindow;

    private User m_User;
    public User User
    {
        get {
            if (m_User == null) {
                m_User = transform.AddComponent<User>();
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
                var game = GameObject.Find("Game");
                if (game != null) {
                    m_GameController = game.transform.GetComponent<GameController>();
                }
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
        DontDestroyOnLoad(GameObject.Find("Camera"));
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
    }

    void Start()
    {
        //初始化配置文件
        CsvManager.ReadCsvs();
        CONFIG.InitDatas();

        //加载数据类
        DataCenter.Init();

        m_TipWindow = UIManager.LoadWindow("Prefab/UI/TipWindow", UIManager.TIP).GetComponent<TipWindow>();
        UIManager.LoadWindow("Prefab/UI/NetWindow", UIManager.TIP).GetComponent<NetWindow>();

        StartCoroutine("SYNC");
    }

    IEnumerator SYNC()
    {
        Platform.Instance.INIT(()=>{
            //加载账号数据
            User.Sync();
        });


        NavigationController.GotoLoading();
        
        yield return null; 
    }

    public void FlyTip(string text)
    {
        if (m_TipWindow != null) {
            m_TipWindow.FlyTip(text);
        }
    }

    public void Popup(string des, Action confirm_callback, Action cancel_callback = null)
    {
        var window = UIManager.LoadWindow("Prefab/UI/PopUpWindow", UIManager.BOARD).GetComponent<PopUpWindow>();
        window.Init(des, confirm_callback, cancel_callback);
    }
}
