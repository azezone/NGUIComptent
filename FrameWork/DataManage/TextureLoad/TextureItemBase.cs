using BestHTTP;
using MyJsonClasses;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextureItemBase : MonoBehaviour
{
    public RawImage mImage;

    private string mImagelUrl = string.Empty;   //图片地址
    private string mRealPath = string.Empty;   //真实路径
    private string mParam = string.Empty;   //参数
    private bool misLocal = false;  //是否为本地图片

    public void SetTexData(string url, string param, bool islocal = false)
    {
        SetDefaultImg();

        StopCoroutine(LoadImgCoroutine());

        this.mImagelUrl = url;
        this.mParam = param;
        this.misLocal = islocal;

        if (string.IsNullOrEmpty(mImagelUrl))
        {
            Debuger.Log("LoadTexture: url is null!");
            return;
        }

        // 先从内存中找，若找不到再去加载
        Texture tex = TextureManager.Instance.GetTexture(mImagelUrl);
        if (tex == null)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        if (misLocal)
        {
            AndroidLoadLocalvideoImg(mImagelUrl, mParam);
        }
        else
        {
            AndroidLoadImg(mImagelUrl);
        }
#elif UNITY_EDITOR
            LoadTexByBestHttp(mImagelUrl);
#endif
        }
        else
        {
            mImage.texture = tex;
        }
    }

    private void AndroidLoadLocalvideoImg(string imgurl, string videourl)
    {
        string[] tParam = new string[4];
        tParam[0] = this.gameObject.name;
        tParam[1] = "LoadImg";
        tParam[2] = imgurl;
        tParam[3] = videourl;

        AndroidInterface.CallAndroidFunction("getLocalVideoThumbnail", tParam);
    }

    private void AndroidLoadImg(string url)
    {
        string[] tParam = new string[4];
        tParam[0] = this.gameObject.name;
        tParam[1] = "LoadImg";
        tParam[2] = url;
        tParam[3] = mParam;

        AndroidInterface.CallAndroidFunction("imageRequest", tParam);
    }

    private void LoadTexByBestHttp(string url)
    {
        mRealPath = url;
        new HTTPRequest(new Uri(mRealPath), (request, response) =>
        {
            var tex = new Texture2D(0, 0);
            tex.LoadImage(response.Data);

            TextureManager.Instance.Cache(mImagelUrl, tex);
            mImage.texture = tex;

        }).Send();
    }

    private void LoadImg(string imageResponseJson)
    {
        Debuger.Log("[TextureItemBase] LoadImg :" + imageResponseJson);
        JImageResponseJson response = JsonUtility.FromJson<JImageResponseJson>(imageResponseJson);

        if (!mImagelUrl.Equals(response.url))
        {
            Debuger.Log(string.Format("path not match mImagelUrl:{0} response.url:{1}", mImagelUrl, response.url));
        }
        else
        {
            Texture tex = TextureManager.Instance.GetTexture(mImagelUrl);
            if (tex == null)
            {
                mRealPath = response.uri;
                Debuger.LogError("Unity LoadImg callback mRealPath:" + mRealPath);
                StartCoroutine(LoadImgCoroutine());
            }
            else
            {
                mImage.texture = tex;
            }
        }
    }

    private IEnumerator LoadImgCoroutine()
    {
        if (!string.IsNullOrEmpty(mRealPath))
        {
            WWW www = new WWW(mRealPath);
            yield return www;
            if (www != null && www.error == null)
            {
                Texture2D tex = new Texture2D(0, 0, TextureFormat.ETC_RGB4, true);
                tex.LoadImage(www.bytes);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Apply();

                TextureManager.Instance.Cache(mImagelUrl, tex);
                mImage.texture = tex;

                www.Dispose();
                www = null;
            }
            else
            {
                Debuger.Log("Unity LoadImg error");
                SetDefaultImg();
            }
        }
        else
        {
            Debuger.Log("Unity LoadImgCoroutine: imgPath is null");
            SetDefaultImg();
        }
    }

    public void SetDefaultImg()
    {
        if (mImage != null && mImage.texture != TextureManager.Instance.DefaultTexture)
        {
            //卸载掉上一张图
            TextureManager.Instance.Unload(mImagelUrl);
            mImage.texture = TextureManager.Instance.DefaultTexture;
        }
    }

    private void OnDestroy()
    {
        if (mImage != null && mImage.texture != TextureManager.Instance.DefaultTexture)
        {
            //卸载掉上一张图
            TextureManager.Instance.Unload(mImagelUrl);
        }
    }
}