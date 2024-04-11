using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace CB
{
    public enum GroundType
    {
        Ground,
        Accelerator,
        BaffleLeft,
        BaffleRight,
        Board
    }

    /// <summary>
    /// 定义多选属性
    /// </summary>
    public class EnumMultiAttribute : PropertyAttribute 
    {

    }

    public class Ground : MonoBehaviour
    {
        [EnumMultiAttribute]
        public GroundType GroundType;



        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        //只处理弹珠碰撞的情况
        void OnCollisionEnter2D(Collision2D collision)
        {
            var ball = collision.transform.GetComponent<Ball>();

            if (ball == null) {
                return;
            }

            //吞掉小球
            if ( ball.Type == _C.BALLTYPE.SMALL) {
                ball.Dead();
                return;
            }

            if (ball.IsGroundValid != true) {
                return;
            }
            

            switch (GroundType)
            {
                case GroundType.Ground:
                    ball.Recyle();

                    if (ball.RunningDirection < 0) {
                        ball.Velocity = new Vector2(-10, 0);
                    } else {
                        ball.Velocity = new Vector2(10, 0);
                    }
                    
                    break;

                    case GroundType.Accelerator:
                    ball.Recyle();
                    ball.Velocity = new Vector2(0, 25);
                    break;

                    case GroundType.BaffleLeft:
                    ball.Recyle();
                    ball.Velocity = new Vector2(10, 0);
                    break;

                    case GroundType.BaffleRight:
                    ball.Recyle();
                    ball.Velocity = new Vector2(-10, 0);
                    break;

                    case GroundType.Board:
                    if (ball.IsRecycle == true) {
                        ball.Show(false);
                        GameFacade.Instance.Game.BreechBall(ball);
                    }
                    break;
                default:
                    break;
            }
            
        }
    }


}