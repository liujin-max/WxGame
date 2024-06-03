using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryWindow : MonoBehaviour
{
    [SerializeField] Button m_BtnContinue;

    // Start is called before the first frame update
    void Start()
    {
        m_BtnContinue.onClick.AddListener(()=>{
            Field.Instance.Dispose();
            Field.Instance.Enter(Field.Instance.Stage.ID + 1);

            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }
}
