using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace CB
{
    public class LoadingWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI c_Score;
        [SerializeField] private TextMeshProUGUI c_Coin;

        [SerializeField] private Button BtnEnter;
        [SerializeField] private Button BtnRecord;


        // Start is called before the first frame update
        void Start()
        {

            c_Score.text    = GameFacade.Instance.DataManager.Score.ToString() + " 层";
            // c_Coin.text     = GameFacade.Instance.DataManager.Coin.ToString() + " <sprite=1>";

            BtnEnter.onClick.AddListener(() => {
                GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

                // GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/GhostUpgradeWindow", GameFacade.Instance.UIManager.BOARD);

                
                GameFacade.Instance.ScenePool.LoadSceneAsync("Game", () => {
                    GameFacade.Instance.Game.Enter();
                });

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });

            BtnRecord.onClick.AddListener(()=>{
                GameFacade.Instance.DataManager.ClearRecord();

                ToolUtility.ApplicationQuit();
            });
        }

    }
}
