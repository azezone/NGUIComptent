using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIHoleCubeTexture))]
public class SplashFrame : MonoBehaviour
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
        if (tex == null)
        {
            tex = gameObject.GetComponent<UIHoleCubeTexture>();
        }
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

    public static SplashFrame Create(Transform trans)
    {
        GameObject go = Instantiate(Resources.Load("SplashImage")) as GameObject;
        Transform tran = go.transform;
        tran.parent = trans.parent;
        tran.localPosition = trans.localPosition;
        Debug.LogError("locapos:"+ trans.localPosition+ " localscale:"+ trans.localScale);

        SplashFrame fram = go.GetComponent<SplashFrame>();
        fram.SetSize(10,10);
        tran.localScale = 0.1f*trans.localScale;
        return fram;
    }

    public void SetSize(int width,int height)
    {
        if (tex == null)
        {
            tex = gameObject.GetComponent<UIHoleCubeTexture>();
            tex.width = width;
            tex.height = height;
        }
    }
}
