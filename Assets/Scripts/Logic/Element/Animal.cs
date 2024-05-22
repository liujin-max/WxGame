using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PC
{
    //动物的基类
    public class Animal
    {
        private AnimalData m_Data;

        public int ID { get {return m_Data.ID;}}
        public string Name { get {return m_Data.Name;}}
        public int Value { get {return m_Data.Value;}}
        public int Belong { get {return m_Data.Belong;}}
        public _C.ANIMAL Type{ get {return m_Data.Type;}}

        public _C.SIDE SIDE;
        public int m_Angle;
        public int X;
        public int Y;


        public bool ValidFlag = true;

        public Animal(AnimalData data, _C.SIDE side = _C.SIDE.NEUTRAL)
        {
            m_Data  = data;
            SIDE    = side;
        }

        public void SetPos(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    public class Dog : Animal
    {
        //狗的5种朝向
        public List<Vector2>[] m_Directions =  new List<Vector2>[5];

        public Dog(AnimalData data, _C.SIDE side = _C.SIDE.NEUTRAL) : base(data, side)
        {
            m_Directions[0] = new List<Vector2>();
            m_Directions[0].Add(new Vector2(0, 1));

            m_Directions[1] = new List<Vector2>();
            m_Directions[1].Add(new Vector2(-1, 0));
            m_Directions[1].Add(new Vector2( 1, 0));

            m_Directions[2] = new List<Vector2>();
            m_Directions[2].Add(new Vector2( 1, 1));
            m_Directions[2].Add(new Vector2( 1, 0));
            m_Directions[2].Add(new Vector2( 1,-1));

            m_Directions[3] = new List<Vector2>();
            m_Directions[3].Add(new Vector2( 0, 1));
            m_Directions[3].Add(new Vector2( 0,-1));
            m_Directions[3].Add(new Vector2( 1, 0));
            m_Directions[3].Add(new Vector2(-1, 0));

            m_Directions[4] = new List<Vector2>();
            m_Directions[4].Add(new Vector2( 1, 1));
            m_Directions[4].Add(new Vector2( 1,-1));
            m_Directions[4].Add(new Vector2(-1, 1));
            m_Directions[4].Add(new Vector2(-1,-1));
        }

        public List<Vector2> GetFocusDirections()
        {
            List<Vector2> result = new List<Vector2>();

            foreach (Vector2 direction in m_Directions[this.ID])
            {
                // 将原始向量旋转90度
                Quaternion rotation = Quaternion.Euler(0, 0, m_Angle);
                Vector2 rotatedVector = rotation * direction;

                result.Add(rotatedVector);
            }

            return result;
        }
    }

    public class Cat : Animal
    {
        public Cat(AnimalData data, _C.SIDE side) : base(data, side)
        {
        }
    }

    public class Mouse : Animal
    {
        public Mouse(AnimalData data, _C.SIDE side = _C.SIDE.NEUTRAL) : base(data, side )
        {
        }
    }
}

