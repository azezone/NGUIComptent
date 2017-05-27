using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    private Texture _defaultTexture = null;
    public Texture DefaultTexture
    {
        get
        {
            if (_defaultTexture == null)
            {
                _defaultTexture = Resources.Load<Texture>("imgdefault0");
            }
            return _defaultTexture;
        }
    }

    private static TextureManager _instance = null;
    public static TextureManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TextureManager");
                GameObject.DontDestroyOnLoad(go);

                _instance = go.AddComponent<TextureManager>();
            }

            return _instance;
        }
    }

    private TexLRUCache _texCache = null;
    private int _maxCacheNum = 50;
    private bool _isInit = false;

    private void Awake()
    {
        this.Init();
    }

    private void Init()
    {
        if (!_isInit)
        {
            _texCache = new TexLRUCache(_maxCacheNum);
            _isInit = true;
        }
    }

    public void Cache(string path, Texture tex)
    {
        TextureData data = new TextureData();
        data.tex = tex;
        _texCache.Set(path, data);
    }

    public void Unload(string path)
    {
        if (_texCache != null)
        {
            _texCache.Unload(path);
        }
    }

    public Texture GetTexture(string path)
    {
        if (_texCache != null)
        {
            TextureData data = _texCache.Get(path);
            if (data != null && data.tex != null)
            {
                return data.tex;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}

public class TextureData
{
    public Texture tex;
    public int usedTimes;          //使用次数
}