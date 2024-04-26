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
        [SerializeField] private GameObject m_MainPivot;
        [SerializeField] private GameObject m_ScorePivot;
        [SerializeField] private TextMeshProUGUI ScoreText;

        private GameObject m_Effect;

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

        public void Init(int stage, bool is_new_score)
        {
            ScoreText.text  = stage.ToString() + "层";

            if (is_new_score)
            {
                GameFacade.Instance.SoundManager.Load(SOUND.NEWSCORE);

                m_Effect = GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.NEWSCORE, m_ScorePivot.transform.position + new Vector3(3.5f, 0, 0));
                GameFacade.Instance.EffectManager.LoadUIEffect(EFFECT.RAINBOW,  m_MainPivot.transform.position + new Vector3(0, 1f, 0));
            }
            else
            {
                GameFacade.Instance.SoundManager.Load(SOUND.RESULT);
            }
            
        }

        void OnDestroy()
        {
            if (m_Effect != null) {
                Destroy(m_Effect);
                m_Effect = null;
            }
        }
    }
}

