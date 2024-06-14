using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoseWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnReturn;
    [SerializeField] Button m_BtnRestart;

    private CDTimer m_Timer = new CDTimer(1.5f);

    void Awake()
    {
        m_BtnReturn.gameObject.SetActive(false);
        m_BtnRestart.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BtnRestart.onClick.AddListener(()=>{
            int level   = Field.Instance.Stage.ID;
            var json    = GameFacade.Instance.DataCenter.Level.GetStageJSON(level);

            if (!GameFacade.Instance.DataCenter.Level.IsFoodEnough2Next(json)) {
                EventManager.SendEvent(new GameEvent(EVENT.UI_POPUPTIP, "体力不足"));
                return;
            }

            //扣除体力
            GameFacade.Instance.DataCenter.User.UpdateFood(-json.Food);


            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                Field.Instance.Enter(level);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });     
        });

        m_BtnReturn.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Leave();
                
                NavigationController.GotoLogin();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });  
        });
    }

    public void Init()
    {
        FlushUI();
    }

    void FlushUI()
    {
        var json = GameFacade.Instance.DataCenter.Level.GetStageJSON(Field.Instance.Stage.ID);

        if (json == null) {
            return;
        }

        int food = json.Food;

        if (food == 0) {
            m_BtnRestart.transform.Find("CostPivot").gameObject.SetActive(false);
            return;
        }

        if (GameFacade.Instance.DataCenter.User.Food >= food)  {
            m_BtnRestart.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text = food.ToString();
        }
        else m_BtnRestart.transform.Find("CostPivot/Cost").GetComponent<TextMeshProUGUI>().text = "<#FF0000>" + food.ToString();
    }

    void Update()
    {
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished())
        {
            m_BtnReturn.gameObject.SetActive(true);
            m_BtnRestart.gameObject.SetActive(true);
        }
    }
}
