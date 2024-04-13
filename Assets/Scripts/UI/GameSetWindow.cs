using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetWindow : MonoBehaviour
{
    // [SerializeField] Button c_Mask;
    [SerializeField] TextMeshProUGUI c_DemageInput;

    [SerializeField] TextMeshProUGUI c_Value;

    [SerializeField] Slider c_MusicSlider;
    [SerializeField] Slider c_SoundSlider;

    [SerializeField] Button c_BtnContinue;
    [SerializeField] Button c_BtnRestart;

    void Awake()
    {
        c_MusicSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.DataManager.MusicVolume = value;
        });

        c_SoundSlider.onValueChanged.AddListener((float value)=>{
            GameFacade.Instance.DataManager.SoundVolume = value;
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

    // Start is called before the first frame update
    void Start()
    {
        c_Value.text = GameFacade.Instance.Game.Stage.ToString();

        c_MusicSlider.value = GameFacade.Instance.DataManager.MusicVolume;
        c_SoundSlider.value = GameFacade.Instance.DataManager.SoundVolume;
    }


}
