using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CB
{
   public class BallSimulator : Ball
    {
        public Vector3 m_collision_point1;
        public Vector3 m_collision_vec1;

        public Vector3 m_collision_point2;
        public Vector3 m_collision_vec2;



        public BallSimulator()
        {
            m_IsSimulate = true;
        }

        public override void Shoot(Vector3 pos)
        {
            base.Shoot(pos);

            m_collision_point1  = _C.VEC3INVALID;
            m_collision_point2  = _C.VEC3INVALID;
        }

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            
            //记录第一次碰撞点
            if (m_collision_point1 == _C.VEC3INVALID) {
                m_collision_point1  = transform.localPosition;
                m_collision_vec1    = collision.contacts[0].point;

            } else if (m_collision_point2 == _C.VEC3INVALID) {
                m_collision_point2  = transform.localPosition;
                m_collision_vec2    = collision.contacts[0].point;
            }
            
        }
    }
 
}
