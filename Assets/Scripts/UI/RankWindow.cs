using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;


namespace CB
{
    public class RankWindow : MonoBehaviour
    {
        [SerializeField] private Button Mask;
        [SerializeField] private RawImage RankImg;



        void Awake()
        {
            Mask.onClick.AddListener(()=>{
                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            WX.ShowOpenData(RankImg.texture, 200, 1360, 680, 860);
            WXUtility.ShowGroupRank();
        }


        void OnDestroy()
        {
            WX.HideOpenData();
        }
    }
}

