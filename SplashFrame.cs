using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIHoleCubeTexture))]
public class testTexture : MonoBehaviour
{
    public enum Speed
    {
        Low   = 0,
        Mid   = 1,
        Hight = 2,
    }
    public Speed speed = Speed.Mid;
    private UIHoleCubeTexture tex;

    void Start()
    {
        tex = gameObject.GetComponent<UIHoleCubeTexture>();
    }

    void Update()
    {
        if (tex != null)
        {
            switch (speed)
            {
                case Speed.Low:
                    tex.CurStep += 5;
                    break;
                case Speed.Mid:
                    tex.CurStep += 10;
                    break;
                case Speed.Hight:
                    tex.CurStep += 20;
                    break;
                default:
                    break;
            }
        }
    }
}
