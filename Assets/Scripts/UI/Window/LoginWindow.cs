using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    [SerializeField] private Text m_Coin;
    [SerializeField] private Text m_Food;

    [Header("按钮")]
    [SerializeField] private Button m_BtnStage;
    [SerializeField] private Button m_BtnEndless;
    [SerializeField] private Button m_BtnSetting;
    

    void Start()
    {
        m_BtnStage.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
            Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);

            //进入游戏
            StageJSON json = NavigationController.GotoGame();
            if (json != null) {
                if (json.Food > 0) {
                    var e = GameFacade.Instance.EffectManager.Load(EFFECT.FLYFOOD, m_BtnStage.transform.position);
                    e.GetComponent<FlyFood>().SetValue(-json.Food);
                }
                

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            }
        });

        m_BtnEndless.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
            Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);

            if (NavigationController.GotoEndless() == true) {
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            }

            
        });

        m_BtnSetting.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
            Platform.Instance.VIBRATE(_C.VIBRATELEVEL.LIGHT);

            GameFacade.Instance.UIManager.LoadWindow("SettingWindow", UIManager.BOARD);
        });
    }


    public void Init()
    {
        m_Coin.text = GameFacade.Instance.DataCenter.User.Coin.ToString();
        m_Food.text = GameFacade.Instance.DataCenter.User.Food.ToString();  // + "/" + _C.DEFAULT_FOOD;


        FlushCost();
        FlushEndless();

        StartCoroutine(Entry());
    }

    public void FlushCost()
    {
        GameObject pivot = m_BtnStage.transform.Find("CostPivot").gameObject;

        var stage_json = GameFacade.Instance.DataCenter.Level.GetStageJSON(GameFacade.Instance.DataCenter.User.Level + 1);

        //已通关
        if (stage_json == null) {
            pivot.gameObject.SetActive(false);
            return;
        }

        int food = stage_json.Food;

        
        if (food > 0) {
            pivot.gameObject.SetActive(true);

            if (GameFacade.Instance.DataCenter.User.Food >= food) 
            {
                pivot.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = food.ToString();
            }
            else pivot.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "<#FF0000>" + food.ToString();
        } else {
            pivot.SetActive(false);
        }
        
    }

    public void FlushEndless()
    {
        if (GameFacade.Instance.DataCenter.User.Level < _C.ENDLESS_UNLOCK_LEVEL)
        {
            m_BtnEndless.GetComponent<ImageGray>().TurnGray(true);
            m_BtnEndless.transform.Find("Text").GetComponent<ImageGray>().TurnGray(true);

            m_BtnEndless.transform.Find("LockPivot").gameObject.SetActive(true);
            m_BtnEndless.transform.Find("LockPivot/Tip").GetComponent<TextMeshProUGUI>().text = string.Format("通过关卡{0}", _C.ENDLESS_UNLOCK_LEVEL);
        }
        else
        {
            m_BtnEndless.GetComponent<ImageGray>().TurnGray(false);
            m_BtnEndless.transform.Find("Text").GetComponent<ImageGray>().TurnGray(false);

            m_BtnEndless.transform.Find("LockPivot").gameObject.SetActive(false);
        }
    }

    IEnumerator Entry()
    {
        m_BtnStage.transform.localPosition      = new Vector3(850, 100, 0);
        m_BtnEndless.transform.localPosition    = new Vector3(850, -155, 0);
        m_BtnSetting.transform.localPosition    = new Vector3(850, -410, 0);

        yield return new WaitForSeconds(0.2f);

        m_BtnStage.transform.DOLocalMoveX(-75, 0.3f);
        m_BtnEndless.transform.DOLocalMoveX( 75, 0.6f);
        m_BtnSetting.transform.DOLocalMoveX(225, 0.9f);

        yield return null;
    }
}
