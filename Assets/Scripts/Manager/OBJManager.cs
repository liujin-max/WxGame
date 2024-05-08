using System.Collections;
using System.Collections.Generic;
using CB;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OBJManager: MonoBehaviour
{
    private Transform PoolLayer;

    private Dictionary<int, List<GameObject>> m_ObstaclePool = new Dictionary<int, List<GameObject>>();

    //特效池
    private Dictionary<string, List<GameObject>> m_EffectPool = new Dictionary<string, List<GameObject>>();


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


    public GameObject AllocateEffect(string effect_path, Vector3 pos)
    {
        GameObject effect = null;

        List<GameObject> effect_list;
        if (m_EffectPool.TryGetValue(effect_path, out effect_list) != true)
        {
            effect_list = new List<GameObject>();
            m_EffectPool.Add(effect_path, effect_list);
        }


        if (effect_list.Count > 0)
        {
            effect = effect_list[0];
            effect.transform.SetParent(SceneManager.GetActiveScene().GetRootGameObjects()[0].transform);
            effect.transform.localPosition = pos;
            effect_list.RemoveAt(0);
        } else {
            // Debug.Log("AllocateEffect 3");
            effect = Instantiate(Resources.Load<GameObject>(effect_path), pos, Quaternion.identity);
        }


        Effect effect_cs    = effect.GetComponent<Effect>();
        effect_cs.ResPath   = effect_path;
        effect_cs.Restart();
        effect.SetActive(true);


        return effect;
    }

    public void RecycleEffect(Effect effect)
    {
        List<GameObject> list = m_EffectPool[effect.ResPath];
        if (list.Count >= 3) {  //只存3个
            Destroy(effect.gameObject);
            return;
        }
        list.Add(effect.gameObject);

        effect.transform.SetParent(PoolLayer);
        effect.transform.localPosition = Vector3.zero;
        effect.gameObject.SetActive(false);
    }
}
