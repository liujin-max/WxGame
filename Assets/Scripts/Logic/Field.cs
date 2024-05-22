using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PC
{
    public class Field : MonoBehaviour
    {
        private Animal[,] m_Animals = new Animal[_C.DEFAULT_WEIGHT, _C.DEFAULT_HEIGHT];

        private Dictionary<_C.SIDE, int> m_SideScores = new Dictionary<_C.SIDE, int>();


        private GameWindow m_GameUI;

        private static Field m_Instance;
        public static Field Instance {
            get  {
                return m_Instance;
            }
        }


        void Awake()
        {
            m_Instance = this;
        }

        //进入
        public void Enter()
        {
            InitAnimals();

            //重置分数
            m_SideScores[_C.SIDE.OUR]   = 0;
            m_SideScores[_C.SIDE.ENEMY] = 0;

            m_GameUI = GameFacade.Instance.UIManager.LoadWindow("GameWindow", GameFacade.Instance.UIManager.BOTTOM).GetComponent<GameWindow>();
            m_GameUI.InitCards(m_Animals);
        }

        void InitAnimals()
        {
            List<Animal> animals = new List<Animal>();

            //添加5张狗
            GameFacade.Instance.DataCenter.GetDatasByType(_C.ANIMAL.DOG).ForEach(data => {
                animals.Add(new Dog(data));
            });

            //10张老鼠牌
            GameFacade.Instance.DataCenter.GetDatasByType(_C.ANIMAL.MOUSE).ForEach(data => {
                animals.Add(new Mouse(data));
            });

            GameFacade.Instance.DataCenter.GetDatasByBelong(100).ForEach(data => {
                animals.Add(new Cat(data, _C.SIDE.OUR));
            });

            GameFacade.Instance.DataCenter.GetDatasByBelong(110).ForEach(data => {
                animals.Add(new Cat(data, _C.SIDE.ENEMY));
            });

            RandomUtility.Shuffle(animals);

            for (int i = 0; i < animals.Count; i++) {
                int x = i % 5;
                int y = i / 5;
                m_Animals[x, y] = animals[i];
            }

        }

        public int GetScore(_C.SIDE side)
        {
            return m_SideScores[side];
        }

        public void Settle()
        {
            StartCoroutine(Settlement());
        }

        //结算分数
        IEnumerator  Settlement()
        {
            for (int i = 0; i < _C.DEFAULT_WEIGHT; i++) {
                for (int j = 0; j < _C.DEFAULT_HEIGHT; j++) {
                    var animal = m_Animals[i, j];

                    //根据狗的朝向，盖上猫牌
                    if (animal.Type == _C.ANIMAL.DOG) 
                    {

                    }

                    //结算老鼠
                    if (animal.Type == _C.ANIMAL.MOUSE)
                    {
                        List<Vector2> vector2s = new List<Vector2>();
                        vector2s.Add(new Vector2(-1, 0));
                        vector2s.Add(new Vector2( 1, 0));
                        vector2s.Add(new Vector2( 0, 1));
                        vector2s.Add(new Vector2( 0, -1));

  
                        List<Animal> round_animals = this.GetDirectionAnimals(new Vector2(i, j), vector2s);
                        if (round_animals.Count > 0) {
                            int temp_our_score  = 0;
                            int temp_enemy_score= 0;
                            foreach (Animal round in round_animals) {
                                if (round.Type == _C.ANIMAL.CAT) {
                                    if (round.SIDE == _C.SIDE.OUR) temp_our_score += round.Value;
                                    else temp_enemy_score += round.Value;
                                }
                            }

                            if (temp_our_score == 0 && temp_enemy_score == 0) continue;

                            if (temp_our_score >= temp_enemy_score) m_SideScores[_C.SIDE.OUR] += animal.Value;
                            else m_SideScores[_C.SIDE.ENEMY] += animal.Value;
                        }

                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }

            yield return null;
        }

        private List<Animal> GetDirectionAnimals(Vector2 pos, List<Vector2> directions)
        {
            List<Animal> animals = new List<Animal>();

            foreach (var vector2 in directions)
            {
                if (vector2.x + pos.x < 0 || vector2.x + pos.x > 4) continue;
                if (vector2.y + pos.y < 0 || vector2.y + pos.y > 4) continue;

                Vector2 t_pos = pos + vector2;
                var animal = m_Animals[(int)t_pos.x, (int)t_pos.y];
                animals.Add(animal);
            }


            return animals;
        }
    }
}

