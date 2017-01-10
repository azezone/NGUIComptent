using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 一个存放任意类型数据的容器
/// </summary>
public class Bundle
{
    private Dictionary<string, BundleData> mdataDict = new Dictionary<string, BundleData>();
    private class BundleData
    {
        public Type type;
        public System.Object obj;

        public BundleData(Type ty, System.Object ob)
        {
            type = ty;
            obj = ob;
        }
    }

    /// <summary>
    /// 设置bundle的数据
    /// 若key相同，后一个会覆盖上一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="val"></param>
    public void SetValue<T>(string key, T val)
    {
        if (mdataDict.ContainsKey(key))
        {
            Debug.Log("the key :" + key + " is already exist!");
            mdataDict.Remove(key);
        }
        BundleData data = new BundleData(val.GetType(), val);
        mdataDict.Add(key, data);
    }

    /// <summary>
    /// 获取bundle中的数据
    /// 在此之前应该先用Contains方法检测是否存在此数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetValue<T>(string key)
    {
        T val = default(T);
        BundleData data;
        if (mdataDict.TryGetValue(key, out data) && (data.obj is T))
        {
            val = (T)data.obj;
        }
        else
        {
            Debug.Log("can not get the key:" + key);
        }

        return val;
    }

    /// <summary>
    /// 检测是否存在此数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains<T>(string key)
    {
        BundleData data;
        //&& (data.type is T)
        //T t = default(T);
        if (mdataDict.TryGetValue(key, out data) && (data.obj is T))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
