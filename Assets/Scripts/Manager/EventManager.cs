using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GameEvent
{
    internal string eventName;
    private object[] _Params;

    public GameEvent(string name, params object[] values)
    {
        eventName = name;

        _Params = values;
    }

    public object GetParam(int order)
    {
        if (_Params.Length > order)
            return _Params[order];

        return null;
    }
}

public class EventManager : MonoBehaviour
{
    private Dictionary<string, Action<GameEvent>> m_eventDictionary = new Dictionary<string, Action<GameEvent>>();


    public void AddHandler(string eventName, Action<GameEvent> listener)
    {
        if (!m_eventDictionary.ContainsKey(eventName))
        {
            m_eventDictionary[eventName] = null;
        }
        m_eventDictionary[eventName] += listener;
    }

    public void DelHandler(string eventName, Action<GameEvent> listener)
    {
        if (m_eventDictionary.ContainsKey(eventName))
        {
            m_eventDictionary[eventName] -= listener;
        }
    }

    public void SendEvent(GameEvent gameEvent)
    {
        Action<GameEvent> thisEvent = null;
        if (m_eventDictionary.TryGetValue(gameEvent.eventName, out thisEvent))
        {
            if (thisEvent != null && thisEvent.GetInvocationList().Length > 0) {
                thisEvent.Invoke(gameEvent);
            }
        }
    }
}