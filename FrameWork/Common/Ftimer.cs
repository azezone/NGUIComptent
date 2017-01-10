using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一个定时器
/// </summary>
public class Ftimer : MonoBehaviour
{
    class EventData
    {
        public string key;
        public float delay;
        public float endtime;
        public bool isKeep;
        public System.Action function;
        public EventData(string k, float time, bool keep, System.Action func)
        {
            key = k;
            delay = time;
            isKeep = keep;
            endtime = Time.time + delay;
            function = func;
        }
    }

    private static List<EventData> eList = new List<EventData>();
    private static Dictionary<string, EventData> eDict = new Dictionary<string, EventData>();
    private static Ftimer _instance = null;

    protected static bool isInit = false;
    public static void Init()
    {
        if (isInit)
        {
            return;
        }
        else if (_instance == null)
        {
            GameObject go = new GameObject("Ftimer");
            _instance = go.AddComponent<Ftimer>();
        }
    }

    void Update()
    {
        OprateTimeUpEvent();
    }

    /// <summary>
    /// 注册一个一次性的事件，事件结束后将被移除
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static bool AddEvent(string eventId, float time, System.Action callback)
    {
        Init();
        if (time < 0 || callback == null)
        {
            Debug.LogError("the event is illegal!");
            return false;
        }
        if (eDict.ContainsKey(eventId))
        {
            return false;
        }
        EventData e = new EventData(eventId, time, false, callback);
        eList.Add(e);
        eDict.Add(eventId, e);
        eList.Sort(CompareTime);
        return true;
    }

    /// <summary>
    /// 注册一个持久的事件，此事件执行完之后将不会被移除
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    //public static bool RegistEvent(string eventId, float time, System.Action callback)
    //{
    //    Init();
    //    if (time < 0 || callback == null)
    //    {
    //        Debug.LogError("the event is illegal!");
    //        return false;
    //    }
    //    if (eDict.ContainsKey(eventId))
    //    {
    //        return false;
    //    }
    //    EventData e = new EventData(eventId, time, true, callback);
    //    eList.Add(e);
    //    eDict.Add(eventId, e);
    //    eList.Sort(CompareTime);
    //    return true;
    //}

    public static void Reset(string key)
    {
        if (!eDict.ContainsKey(key))
        {
            Debug.Log("ther is not the key:" + key);
            return;
        }
        else
        {
            EventData data = null;
            eDict.TryGetValue(key, out data);
            data.endtime = Time.time + data.delay;
        }
    }

    public static bool HasTimer(string key)
    {
        if (eDict.ContainsKey(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OprateTimeUpEvent()
    {
        if (eList == null || eList.Count < 1)
        {
            return;
        }

        for (int i = 0; i < eList.Count;)
        {
            if (Time.time >= eList[i].endtime)
            {
                if (eList[i].function != null)
                {
                    try
                    {
                        eList[i].function();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("OprateTimeUpEvent exception:" + ex.ToString());
                        eDict.Remove(eList[i].key);
                        eList.RemoveAt(i);
                        throw;
                    }
                }
                if (!eList[i].isKeep)
                {
                    eDict.Remove(eList[i].key);
                    eList.RemoveAt(i);
                }
            }
            else
            {
                return;
            }
        }
    }

    private static int CompareTime(EventData e1, EventData e2)
    {
        if (e1.endtime < e2.endtime)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
