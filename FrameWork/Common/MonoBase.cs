using UnityEngine;
using System;

public class MonoBase : MonoBehaviour
{
    [NonSerialized]
    public Transform cachetransform;
    [NonSerialized]
    public GameObject cachegameobject;

    void Awake()
    {
        cachetransform = transform;
        cachegameobject = gameObject;
    }
}