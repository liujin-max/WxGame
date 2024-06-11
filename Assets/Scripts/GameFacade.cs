using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GameFacade : MonoBehaviour
{

    public string Version = "1.0.0";

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

    private DisplayEngine m_DisplayEngine = null;
    public DisplayEngine DisplayEngine
    {
        get {
            if (m_DisplayEngine == null) {
                m_DisplayEngine = transform.AddComponent<DisplayEngine>();
            }
            return m_DisplayEngine;
        }
    }

    private TimeManager m_TimeManager = null;
    public TimeManager TimeManager
    {
        get {
            if (m_TimeManager == null) {
                m_TimeManager = transform.AddComponent<TimeManager>();
            }
            return m_TimeManager;
        }
    }

    #endregion

    private static GameFacade _instance = null;
    public static GameFacade Instance
    {
        get {return _instance;} 
    }

    public bool TestMode = false;
    public int TestStage = 0;

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
        // CONFIG.InitDatas();

        //加载数据类
        DataCenter.Init();


        StartCoroutine("SYNC");
    }

    IEnumerator SYNC()
    {
        Platform.Instance.INIT(()=>{
            //加载账号数据
            m_DataCenter.User.Sync();
        });

        UIManager.LoadWindow("TipWindow", UIManager.TIP);
        UIManager.LoadWindow("MaskWindow", UIManager.TIP);

        //进入游戏
        NavigationController.GotoLogin();

        
        yield return null; 
    }


    void Update()
    {
        float dt = Time.deltaTime;

        DataCenter.Update(dt);
    }
}
