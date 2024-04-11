using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.UI;

public class BallItem : MonoBehaviour
{
    [HideInInspector] public Ball m_Ball;


    [SerializeField] private Image c_Icon;


    public void Init(Ball ball)
    {
        m_Ball = ball;
        var config = CONFIG.GetBallData(ball.Type);

        c_Icon.sprite = Resources.Load<Sprite>(config.Icon);
        c_Icon.SetNativeSize();
    }

}
