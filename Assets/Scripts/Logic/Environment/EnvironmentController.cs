using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;



namespace CB
{
    public class EnvironmentController : MonoBehaviour
    {
        private Environment m_Env = null;

        public void OnInit(int stage_order)
        {
            if (stage_order % 5 != 0) {
                return;
            }

            //
            Debug.Log("开始：" + stage_order);

            int rand = RandomUtility.Random(0, 8);


            switch (rand)
            {
                case 0:
                    m_Env = transform.AddComponent<Wind>();
                    break;

                case 1:
                    m_Env = transform.AddComponent<Dark>();
                    break;

                case 2:
                    m_Env = transform.AddComponent<Gravity>();
                    break;

                case 3:
                    m_Env = transform.AddComponent<BBMachine>();
                    break;

                case 4:
                    m_Env = transform.AddComponent<EarthQuake>();
                    break;

                case 5:
                    m_Env = transform.AddComponent<Blizard>();
                    break;

                case 6:
                    m_Env = transform.AddComponent<Rock>();
                    break;

                case 7:
                    m_Env = transform.AddComponent<Scaler>();
                    break;
                
                default:
                    m_Env = transform.AddComponent<Wind>();
                    break;
            }
        }

        public void OnBegin()
        {
            if (m_Env == null) return;

            m_Env.OnEnter();

            var window  = GameFacade.Instance.UIManager.LoadWindow("Prefab/UI/EnvironmentWindow", GameFacade.Instance.UIManager.BOARD).GetComponent<EnvironmentWindow>();
            window.Init(m_Env);
        }


        public void OnEnd()
        {
            if (m_Env != null) {
                m_Env.OnLeave();

                Destroy(m_Env);
            }
            m_Env = null;
        }
    }


    public class Environment : MonoBehaviour
    {
        public string Name = "";

        public virtual void OnEnter()
        {

        }

        public virtual void OnLeave()
        {

        }

        public virtual string GetDescription()
        {
            return "";
        }
    }
}

