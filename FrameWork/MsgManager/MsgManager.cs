using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 消息管理器
/// step1：新建一个消息号
/// step2：在相应模块注册需要订阅的消息（RegistMsg），此模块需继承IMsgHandle
/// step3：在相应的位置发送消息（SendMsg）
/// </summary>
public class MsgManager : Singleton<MsgManager>
{
    private Dictionary<MsgID, List<IMsgHandle>> msgDict = new Dictionary<MsgID, List<IMsgHandle>>();

    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="handle"></param>
    public void RegistMsg(MsgID id, IMsgHandle handle)
    {
        List<IMsgHandle> handlist;
        if (msgDict.ContainsKey(id))
        {
            msgDict.TryGetValue(id, out handlist);
            if (handlist == null)
            {
                handlist = new List<IMsgHandle>();
            }
            handlist.Add(handle);
        }
        else
        {
            handlist = new List<IMsgHandle>();
            handlist.Add(handle);
            msgDict.Add(id, handlist);
        }
    }

    /// <summary>
    /// 移除消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool RemoveMsg(MsgID id, IMsgHandle handle)
    {
        bool result = false;
        if (msgDict.ContainsKey(id))
        {
            List<IMsgHandle> handlelist;
            msgDict.TryGetValue(id, out handlelist);
            if (handlelist != null && handlelist.Count >= 0)
            {
                for (int i = 0; i < handlelist.Count; i++)
                {
                    if (handlelist[i] == handle)
                    {
                        handlelist.RemoveAt(i);
                        return true;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="id"></param>
    public void SendMsg(MsgID id, Bundle bundle)
    {
        List<IMsgHandle> handlist;
        if (msgDict.TryGetValue(id, out handlist))
        {
            if (handlist != null && handlist.Count > 0)
            {
                for (int i = 0; i < handlist.Count; i++)
                {
                    MonoBehaviour mono = handlist[i] as MonoBehaviour;
                    if (mono == null)
                    {
                        Debug.Log("the MonoBehaviour is null!");
                        RemoveMsg(id, handlist[i]);
                    }
                    else
                    {
                        handlist[i].HandleMessage(id, bundle);
                    }
                }
            }
            else
            {
                msgDict.Remove(id);
                Debug.Log("the hand list is null!");
            }
        }
        else
        {
            msgDict.Remove(id);
            Debug.Log("the hand list is null!");
        }
    }
}