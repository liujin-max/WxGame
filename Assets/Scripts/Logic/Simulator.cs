using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



namespace CB
{
    public class Simulator: MonoBehaviour
    {
        private BallSimulator m_Ball;
        private UnityEngine.SceneManagement.Scene m_Scene;
        private PhysicsScene2D m_PhysicsScene;

        private List<GameObject> m_Points = new List<GameObject>();
        private List<GameObject> m_Aims = new List<GameObject>();


        public int FocusCount = 1;

        void Start()
        {
            CreatePhysicsScene();
        }

        private void CreatePhysicsScene()
        {
            m_Ball  = GameObject.Find("BallSimulator").GetComponent<BallSimulator>();
            m_Ball.gameObject.SetActive(false);

            m_Scene = UnitySceneManager.CreateScene("Simulator");
            m_PhysicsScene = m_Scene.GetPhysicsScene2D();
            
            UnitySceneManager.MoveGameObjectToScene(m_Ball.gameObject, m_Scene);
        }

        private GameObject GetPoint(int order)
        {
            if (m_Points.Count > order) {
                return m_Points[order];
            }

            var obj = Instantiate(Resources.Load<GameObject>("Prefab/FocusPoint"), Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            m_Points.Add(obj);

            UnitySceneManager.MoveGameObjectToScene(obj, m_Scene);

            return obj;
        }

        private GameObject GetFocus(int order)
        {
            if (m_Aims.Count > order) {
                return m_Aims[order];
            }

            var obj = Instantiate(Resources.Load<GameObject>("Prefab/FocusAim"), Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            m_Aims.Add(obj);

            UnitySceneManager.MoveGameObjectToScene(obj, m_Scene);

            return obj;
        }

        public void SimulateShoot(Vector3 target_pos)
        {
            GameFacade.Instance.Game.Pause();
            
            m_Ball.gameObject.SetActive(true);

            m_Ball.Shoot(target_pos);
            Simulating();
        }
        
        public void SimulateEnd()
        {
            GameFacade.Instance.Game.Resume();

            m_Ball.gameObject.SetActive(false);

            foreach (var o in m_Points) {
                o.SetActive(false);
            }

            foreach (var o in m_Aims) {
                o.SetActive(false);
            }
        }



        public void Simulating()
        {
            foreach (var o in m_Points) {
                o.SetActive(false);
            }

            foreach (var o in m_Aims) {
                o.SetActive(false);
            }

            int order = 0;
            int index = 0;


            int collision_count = 0;
            while (true)
            {
                m_PhysicsScene.Simulate(0.02f);

                bool flag1 = ToolUtility.Approximately(m_Ball.transform.localPosition, m_Ball.m_collision_point1);
                bool flag2 = ToolUtility.Approximately(m_Ball.transform.localPosition, m_Ball.m_collision_point2);
                if (flag1 || flag2) {
                    var obj = GetFocus(collision_count);
                    obj.SetActive(true);

                    if (flag1)  {
                        obj.transform.localPosition = m_Ball.m_collision_vec1; //m_Ball.transform.localPosition;
                    } else {
                        obj.transform.localPosition = m_Ball.m_collision_vec2; //m_Ball.transform.localPosition;
                    }
                    

                    index = 0;
                    collision_count++;
                    if (collision_count >= FocusCount) {
                        break;
                    }     

                } else {
                    if (index == 3) {
                        var obj = GetPoint(order);
                        obj.SetActive(true);
                        obj.transform.localPosition = m_Ball.transform.localPosition;
                        order++;
                        index = 0;
                    }
                }

                if (order >= 10) {
                    break;
                }

                index++;
            }
        }

    }

}
