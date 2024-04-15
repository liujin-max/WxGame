using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
    public class BallSplit : Ball
    {
        private float m_Count = 4;

        public override string GetDescription()
        {
            var str = string.Format("击中宝石时有概率向四周发射<size=32><#43A600>{0}</color></size>颗分裂小弹珠。", m_Count);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            
            this.OnHitGhost(collision);
            this.OnHitObstable(collision);

            //碰撞的对象是障碍物
            if (collision.gameObject.GetComponent<Obstacle>() != null) {
                if (collision.contacts.Length <= 0){
                    return;
                }

                if (RandomUtility.IsHit(60) == true)
                {
                    GameFacade.Instance.SoundManager.Load(SOUND.SPLIT);

                    Vector3 collision_point = transform.localPosition; //collision.contacts[0].point;
                    //分裂出#个小球
                    for (int i = 0; i < m_Count; i++)
                    {
                        var ball = GameFacade.Instance.Game.PushBall(collision_point, _C.BALLTYPE.SMALL);

                        Vector2 direction = Quaternion.Euler(0, 0, RandomUtility.Random(0, 360)) * Vector2.right;
                        ball.Crash(direction * 25);
                    }
                }
            }
        }
    }
}
