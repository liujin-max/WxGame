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
            //每3关
            if (stage_order % _C.STAGESTEP != 0) {
                return;
            }

            int rand = RandomUtility.Random(0, 12);

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

                case 8:
                    m_Env = transform.AddComponent<Fall>();
                    break;

                case 9:
                    m_Env = transform.AddComponent<FadeShine>();
                    break;

                case 10:
                    m_Env = transform.AddComponent<BanSwitch>();
                    break;

                case 11:
                    m_Env = transform.AddComponent<NOCoin>();
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
        protected bool m_IsEnter = false;

        public virtual void OnEnter()
        {
            m_IsEnter = true;
        }

        public virtual void OnLeave()
        {
            m_IsEnter = false;
        }

        public virtual string GetDescription()
        {
            return "";
        }
    }
}

