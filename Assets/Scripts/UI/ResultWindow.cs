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
        [SerializeField] private Button BtnReborn;
        [SerializeField] private Button BtnShare;
        [SerializeField] private GameObject m_MainPivot;
        [SerializeField] private GameObject m_ScorePivot;
        [SerializeField] private TextMeshProUGUI ScoreText;

        private GameObject m_Effect;
        private CDTimer m_Timer = new CDTimer(0.5f);

        // Start is called before the first frame update
        void Start()
        {
            Platform.Instance.BANNER_VIDEOAD("adunit-76c9b3e9a46fb235", true, 50);

            BtnRestart.onClick.AddListener(()=>{
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);


                GameFacade.Instance.Game.Dispose();
                NavigationController.GotoLoading();
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnReborn.gameObject.SetActive(GameFacade.Instance.Game.RebornTimes <= 0);
            BtnReborn.onClick.AddListener(()=>{
                Platform.Instance.REWARD_VIDEOAD("adunit-a345ac227bfdc83e", ()=>{
                    GameFacade.Instance.Game.RebornTimes++;
                    GameFacade.Instance.Game.Export(true);      //重新上传存档

                    //继续游戏
                    GameFacade.Instance.Game.Resume();

                    //清理障碍物
                    GameFacade.Instance.Game.ClearElements();

                    if (GameFacade.Instance.Game.Stage % _C.STAGESTEP == 0) { //每3关
                        GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_SHOP);
                    } else {
                        GameFacade.Instance.Game.DOTransist(_C.FSMSTATE.GAME_COMPLEX);
                    }

                    GameFacade.Instance.UIManager.UnloadWindow(gameObject);
                });
            });

            BtnShare.onClick.AddListener(()=>{
                Platform.Instance.SHARE();
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

        void Update()
        {
            if (m_Timer != null) {
                m_Timer.Update(Time.deltaTime);
                if (m_Timer.IsFinished() == true) {
                    m_Timer = null;

                    Platform.Instance.INTER_VIDEOAD("adunit-ef2a56ff6235dc71");
                }
            }
        }

        void OnDestroy()
        {
            Platform.Instance.BANNER_VIDEOAD("adunit-76c9b3e9a46fb235", false);

            if (m_Effect != null) {
                Destroy(m_Effect);
                m_Effect = null;
            }
        }
    }
}

