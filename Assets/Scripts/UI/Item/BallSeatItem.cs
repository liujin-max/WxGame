using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallSeatItem : MonoBehaviour
{
    [SerializeField] private Image c_Icon;

    public void Init(int type = -1)
    {
        if (type == -1) {
            c_Icon.gameObject.SetActive(false);
        } else {
            c_Icon.gameObject.SetActive(true);

            var config = CONFIG.GetBallData((_C.BALLTYPE)type);

            c_Icon.sprite = Resources.Load<Sprite>(config.Icon);
            c_Icon.SetNativeSize();
        }
    }
}
