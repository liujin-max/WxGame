using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI c_Score;
        [SerializeField] private TextMeshProUGUI c_Coin;

        [SerializeField] private Button BtnEnter;
        [SerializeField] private Button BtnRank;


        
        // Start is called before the first frame update
        void Start()
        {
            c_Score.text    = GameFacade.Instance.User.Score.ToString() + " 层";

            BtnEnter.onClick.AddListener(() => {
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);
                
                GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                    GameFacade.Instance.Game.Enter();
                });

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnRank.onClick.AddListener(()=>{  
                GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/RankWindow", GameFacade.Instance.UIManager.BOARD);
            });



        }

    }
}
