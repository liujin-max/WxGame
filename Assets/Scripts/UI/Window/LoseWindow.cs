using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnRestart;

    private CDTimer m_Timer = new CDTimer(1.5f);

    void Awake()
    {
        m_BtnRestart.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_BtnRestart.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                Field.Instance.Enter(Field.Instance.Stage.ID);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });     
        });
    }

    void Update()
    {
        m_Timer.Update(Time.deltaTime);
        if (m_Timer.IsFinished())
        {
            m_BtnRestart.gameObject.SetActive(true);
        }
    }
}
