using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI c_Value;

    [SerializeField] Slider c_MusicSlider;
    [SerializeField] Slider c_SoundSlider;
    [SerializeField] Toggle c_VibrateToggle;

    [SerializeField] Button c_BtnContinue;
    [SerializeField] Button c_BtnRestart;

    void Awake()
    {
        Platform.Instance.BANNER_VIDEOAD("adunit-78e1a7920085d132", true);

        c_MusicSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.MusicVolume = value;
        });

        c_SoundSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.SystemManager.SoundVolume = value;
        });

        c_VibrateToggle.onValueChanged.AddListener((flag)=>{
            GameFacade.Instance.SystemManager.VibrateFlag = flag;
        });

        c_BtnContinue.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.Resume();
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });

        c_BtnRestart.onClick.AddListener(()=>{
            GameFacade.Instance.SoundManager.Load(SOUND.CLICK);

            GameFacade.Instance.Game.Restart();
            GameFacade.Instance.Game.Enter();
            GameFacade.Instance.UIManager.UnloadWindow(gameObject);
        });
    }

    void OnDestroy()
    {
        Platform.Instance.BANNER_VIDEOAD("adunit-78e1a7920085d132", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        c_Value.text = GameFacade.Instance.Game.Stage.ToString();

        c_MusicSlider.value     = GameFacade.Instance.SystemManager.MusicVolume;
        c_SoundSlider.value     = GameFacade.Instance.SystemManager.SoundVolume;
        c_VibrateToggle.isOn    = GameFacade.Instance.SystemManager.VibrateFlag;
    }


}
