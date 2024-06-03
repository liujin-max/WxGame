using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;


//动画系统
//负责管理所有的动画节点按序播放
public class DisplayEngine : MonoBehaviour
{   
    //动画轨道
    //有些动画要按顺序播放
    //而有些动画则可以直接播放
    public enum Track
    {
        Common,     //并行
    }

    private Dictionary<Track, List<DisplayEvent>> m_Events = new Dictionary<Track,List<DisplayEvent>>();


    public void Put(Track track, DisplayEvent displayEvent)
    {
        if (!m_Events.ContainsKey(track)) {
            m_Events.Add(track, new List<DisplayEvent>());
        }

        m_Events[track].Add(displayEvent);
    }

    public bool IsIdle()
    {
        bool is_clear = true;
        foreach (var item in m_Events) {
            List<DisplayEvent> events = item.Value;
            if (events.Count > 0) {
                is_clear = false;
                break;
            }
        }

        return is_clear;
    }

    void Update()
    {
        float dealta_time = Time.deltaTime;

        foreach (var item in m_Events)
        {
            List<DisplayEvent> _removes = new List<DisplayEvent>();
            item.Value.ForEach(e => {
                if (e.IsIdle()) {
                    e.Start();
                } else if (e.IsPlaying()) {
                    e.Update(dealta_time);
                }

                if (e.IsFinished() == true) {
                    _removes.Add(e);
                }
            });

            _removes.ForEach(e => {
                item.Value.Remove(e);
            });
        }
    }
}
