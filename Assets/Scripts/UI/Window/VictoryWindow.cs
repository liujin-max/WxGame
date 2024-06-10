using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnContinue;


    private CDTimer m_StarTimer = new CDTimer(0.1f);

    // Start is called before the first frame update
    void Start()
    {
        m_BtnContinue.onClick.AddListener(()=>{
            GameFacade.Instance.EffectManager.Load(EFFECT.SWITCH, Vector3.zero, UIManager.EFFECT.gameObject).GetComponent<SceneSwitch>().Enter(()=>{
                Field.Instance.Dispose();
                Field.Instance.Enter(Field.Instance.Stage.ID + 1);

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });     
        });
    }


    void Update()
    {
        m_StarTimer.Update(Time.deltaTime);
        if (m_StarTimer.IsFinished() == true)
        {
            m_StarTimer.Reset(RandomUtility.Random(300, 700) / 1000.0f);


            
            float pos_x     = RandomUtility.Random(-50000, 50000) / 100.0f;
            float pos_y     = RandomUtility.Random(-25000, 25000) / 100.0f;
            Vector3 pos     = new Vector3(pos_x, 464 + pos_y, 0) / 100.0f;

            GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.SHINESTAR, pos);
        }
    }
}
