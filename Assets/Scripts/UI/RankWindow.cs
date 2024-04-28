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
        [SerializeField] private GameObject Box;


        void Awake()
        {
            Mask.onClick.AddListener(()=>{
                Box.SetActive(false);
                WX.HideOpenData();

                GameFacade.Instance.UIManager.UnloadWindow(gameObject);
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            Box.SetActive(true);

            WX.ShowOpenData(RankImg.texture, 200, 480, 680, 860);

            WXUtility.ShowFriendsRank();
        }
    }
}

