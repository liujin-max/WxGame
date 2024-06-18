using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyWindow : MonoBehaviour
{
    [SerializeField] private Transform m_Content;

    [SerializeField] private Button m_BtnClose;

    // Start is called before the first frame update
    void Start()
    {
        m_BtnClose.onClick.AddListener(()=>{
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    public void Init()
    {
        InitTasks();
    }

    void InitTasks()
    {
        GameFacade.Instance.DataCenter.Daily.GetTasks().ForEach(task => {
            var item = GameFacade.Instance.UIManager.LoadItem("DailyItem", m_Content).GetComponent<DailyItem>();
            item.Init(task);
        });
    }
}
