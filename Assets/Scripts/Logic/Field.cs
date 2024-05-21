using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PC
{
    public class Field
    {
        private Animal[,] m_Animals = new Animal[5, 5];




        private GameWindow m_GameUI;

        private static Field m_Instance;
        public static Field Instance {
            get  {
                if (m_Instance == null) {
                    m_Instance = new Field();
                }
                return m_Instance;
            }
        }


        public Field()
        {

        }

        //进入
        public void Enter()
        {
            InitAnimals();

            m_GameUI = GameFacade.Instance.UIManager.LoadWindow("GameWindow", GameFacade.Instance.UIManager.BOTTOM).GetComponent<GameWindow>();
            m_GameUI.InitAnimals(m_Animals);
        }

        void InitAnimals()
        {
            List<Animal> animals = new List<Animal>();

            //添加5张狗
            GameFacade.Instance.DataCenter.GetDatasByType(_C.ANIMAL.DOG).ForEach(data => {
                animals.Add(new Animal(data));
            });

            //10张老鼠牌
            GameFacade.Instance.DataCenter.GetDatasByType(_C.ANIMAL.MOUSE).ForEach(data => {
                animals.Add(new Animal(data));
            });

            GameFacade.Instance.DataCenter.GetDatasByBelong(100).ForEach(data => {
                animals.Add(new Animal(data));
            });

            GameFacade.Instance.DataCenter.GetDatasByBelong(110).ForEach(data => {
                animals.Add(new Animal(data));
            });

            RandomUtility.Shuffle(animals);

            for (int i = 0; i < animals.Count; i++)
            {
                int x = i % 5;
                int y = i / 5;

                m_Animals[x, y] = animals[i];
            }
        }
    }
}

