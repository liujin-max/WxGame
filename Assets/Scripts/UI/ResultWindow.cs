using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CB
{
    public class ResultWindow : MonoBehaviour
    {

        [SerializeField] private Button BtnRestart;

        [SerializeField] private TextMeshProUGUI ScoreText;

        // Start is called before the first frame update
        void Start()
        {
            BtnRestart.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);


                GameFacade.Instance.Game.Dispose();
                NavigationController.GotoLoading();
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        public void Init(int stage, int coin)
        {
            ScoreText.text  = stage.ToString() + "层";

            GameFacade.Instance.SoundManager.Load(SOUND.RESULT);
        }
    }
}

