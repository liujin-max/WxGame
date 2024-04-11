using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;

public class OBJManager: MonoBehaviour
{
    private Transform PoolLayer;

    private Dictionary<int, List<GameObject>> m_ObstaclePool = new Dictionary<int, List<GameObject>>();


    void Awake()
    {
        PoolLayer = GameObject.Find("POOL").transform;
    }


    public GameObject AllocateObstacle(int order)
    {
        GameObject obstacle = null;

        List<GameObject> obstacle_list;
        if (m_ObstaclePool.TryGetValue(order, out obstacle_list) != true)
        {
            obstacle_list = new List<GameObject>();
            m_ObstaclePool.Add(order, obstacle_list);
        }

        if (obstacle_list.Count > 0)
        {
            obstacle = obstacle_list[0];
            obstacle_list.RemoveAt(0);
        } else {
            GameObject prefab = Resources.Load<GameObject>(_C.ObstaclePrefabs[order]);
            obstacle = Object.Instantiate(prefab);
        }

        return obstacle;
    } 

    public void RecycleObstacle(Obstacle obstacle)
    {
        List<GameObject> list = m_ObstaclePool[obstacle.Order];
        list.Add(obstacle.gameObject);

        obstacle.transform.SetParent(PoolLayer);
        obstacle.transform.localPosition = Vector3.zero;
    }
}
