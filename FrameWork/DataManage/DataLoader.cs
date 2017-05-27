using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MyJsonClasses;
using System;

public class DataLoader : MonoBehaviour
{
    private static DataLoader _instance = null;
    public static DataLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("DataLoader");
                GameObject.DontDestroyOnLoad(go);
                _instance = go.AddComponent<DataLoader>();
            }
            return _instance;
        }
    }

    private string tabJFileName = "tab.json";
    private string mainpageJFileName = "mainpage.json";
    private string tabpageJFileName = "tabpage.json";
    private string historyJFileName = "history.json";
    private string downloadJFileName = "download.json";
    private string localvideoJFileName = "localvideo.json";

    private const string mURLMainPage = "/u3d/homePage"; //main page request url
    private const string mURLTab = "/u3d/category";  //tab request url
    private const string mURLTabPage = "/u3d/category/video"; //tab page request url
    private const string mURLTabHistory = "/u3d/history/list"; //tab history request url
    private const string mURLTabDownload = "/u3d/download/list"; //tab download request url
    private const string mURLTabLocal = "/u3d/local/list"; //tab history request url
    private const string mURLSearch = "/v3/video/search";    //search request url
    private const string mURLHotWords = "/video/search/hotKeywords";//hotwords request url
    private const string mURLVideoList = "/v2/video/set";//videolist request url

    private const string mAndroidHttpRequest = "httpRequest";
    private const string mAndroidCancelHttpRequest = "cancelHttpRequest";
    private const string mAndroidImageRequest = "imageRequest";

    /// <summary>
    /// 通过UpdateUIData消息号来更新主页数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="jsondata"></param>
    private void UpdateUIData(string type, string jsondata)
    {
        Debuger.Log(string.Format("[DataLoader] UpdateUIData type:{0} data:{1}", type, jsondata));
        Bundle bundle = new Bundle();
        bundle.SetValue<string>("type", type);
        bundle.SetValue<string>("jsondata", jsondata);
        MsgManager.Instance.SendMsg(MsgID.UpdateUIData, bundle);
    }

    /// <summary>
    /// 获取category数据
    /// </summary>
    public void GetTabJsonData()
    {
        JTabRequest request = new JTabRequest();
        request.request = mURLTab;
        request.tag = mURLTab;
        string requestJson = JsonUtility.ToJson(request);

        string[] tParam = new string[3];
        tParam[0] = this.gameObject.name;
        tParam[1] = "GetTabJsonDataCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        GetTabJsonDataCallBack(LoadJsonStrFromFile(tabJFileName));
#endif
    }

    private void GetTabJsonDataCallBack(string tabJsonData)
    {
        UpdateUIData("tabjsondata", tabJsonData);
    }

    /// <summary>
    /// 获取主页数据
    /// </summary>
    public void GetMainPageJsonData()
    {
        JMainPageRequest request = new JMainPageRequest();
        request.request = mURLMainPage;
        request.tag = mURLMainPage;
        string requestJson = JsonUtility.ToJson(request);

        string[] tParam = new string[3];
        tParam[0] = this.gameObject.name;
        tParam[1] = "GetMainPageJsonDataCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        GetMainPageJsonDataCallBack(LoadJsonStrFromFile(mainpageJFileName));
#endif
    }

    private void GetMainPageJsonDataCallBack(string data)
    {
        UpdateUIData("mainpage", data);
    }

    /// <summary>
    /// 获取某个category下的数据
    /// </summary>
    /// <param name="pageCategory"></param>
    /// <param name="offset"></param>
    public void GetTabPageJsonData(string pageCategory, string offset)
    {
        JPageRequest request = new JPageRequest();
        request.request = mURLTabPage;
        request.tag = Constant.TAB_PAGE_HTTPTAG;
        JPageRequestParams param = new JPageRequestParams();
        param.pageCategory = pageCategory;
        param.offset = offset;
        //param.vRFormat = vRFormat;
        request.param = param;

        string requestJson = JsonUtility.ToJson(request);
        //string funcName = "AddTabPageData";

        string[] tParam = new string[3];
        tParam[0] = gameObject.name;
        tParam[1] = "GetTabPageJsonDataCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        GetTabPageJsonDataCallBack(LoadJsonStrFromFile(tabpageJFileName));
#endif
    }

    private void GetTabPageJsonDataCallBack(string data)
    {
        UpdateUIData("tabpage", data);
    }

    /// <summary>
    /// 获取本地视频数据
    /// </summary>
    public void GetTabLocalJsonData()
    {
        JPageRequest request = new JPageRequest();
        request.request = mURLTabLocal;
        request.tag = Constant.TAB_PAGE_HTTPTAG;
        JPageRequestParams param = new JPageRequestParams();
        param.pageCategory = "";
        param.offset = "";
        //param.vRFormat = vRFormat;
        request.param = param;

        string requestJson = JsonUtility.ToJson(request);
        //string funcName = "AddTabPageDataOnce";

        string[] tParam = new string[3];
        tParam[0] = gameObject.name;
        tParam[1] = "AddTabPageDataOnceCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        AddTabPageDataOnceCallBack(LoadJsonStrFromFile(localvideoJFileName));
#endif
    }

    private void AddTabPageDataOnceCallBack(string data)
    {
        UpdateUIData("tabpageonce", data);
    }

    /// <summary>
    /// 获取已下载数据
    /// </summary>
    public void GetTabDownloadJsonData()
    {
        JPageRequest request = new JPageRequest();
        request.request = mURLTabDownload;
        request.tag = Constant.TAB_PAGE_HTTPTAG;
        JPageRequestParams param = new JPageRequestParams();
        param.pageCategory = "";
        param.offset = "";
        //param.vRFormat = vRFormat;
        request.param = param;

        string requestJson = JsonUtility.ToJson(request);
        //string funcName = "AddTabPageDataOnce";

        string[] tParam = new string[3];
        tParam[0] = gameObject.name;
        tParam[1] = "AddTabPageDataOnceCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        AddTabPageDataOnceCallBack(LoadJsonStrFromFile(downloadJFileName));
#endif
    }

    /// <summary>
    /// 获取历史数据
    /// </summary>
    public void GetTabHistoryJsonData()
    {
        JPageRequest request = new JPageRequest();
        request.request = mURLTabHistory;
        request.tag = Constant.TAB_PAGE_HTTPTAG;
        JPageRequestParams param = new JPageRequestParams();
        param.pageCategory = "";
        param.offset = "";
        //param.vRFormat = vRFormat;
        request.param = param;

        string requestJson = JsonUtility.ToJson(request);
        //string funcName = "AddTabPageDataOnce";

        string[] tParam = new string[3];
        tParam[0] = gameObject.name;
        tParam[1] = "AddTabPageDataOnceCallBack";
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction (mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR || UNITY_IOS
        AddTabPageDataOnceCallBack(LoadJsonStrFromFile(historyJFileName));
#endif
    }

    /// <summary>
    /// 获取搜索结果
    /// </summary>
    /// <param name="keyword"></param>
    public void GetSearchJsonData(string keyword)
    {
        GetSearchJsonData(keyword, gameObject.name, "GetSearchJsonDataCallBack");
    }

    /// <summary>
    /// 获取搜索结果
    /// </summary>
    /// <param name="keyword"></param>
    public void GetSearchJsonData(string keyword, string goname, string funname)
    {
        JSearchRequest request = new JSearchRequest();
        request.request = mURLSearch;
        JSearchRequestParams param = new JSearchRequestParams();
        param.keyWords = keyword;
        request.param = param;

        string requestJson = JsonUtility.ToJson(request);
        string u3dName = goname;
        string funcName = funname;

        string[] tParam = new string[3];
        tParam[0] = u3dName;
        tParam[1] = funcName;
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidInterface.CallAndroidFunction(mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR
        string mainJsonStr = LoadJsonStrFromFile(mainpageJFileName);
        JMainPage page = JsonUtility.FromJson<JMainPage>(mainJsonStr);
        JSearchData data = new JSearchData();
        //page.data.feedStream
        data.videoList = new JFeedItem[20];
        for (int i = 0; i < data.videoList.Length; i++)
        {
            data.videoList[i] = page.data.feedStream[0];
        }
        CacheVoiceSearchData.Instance.FeedList = data;
#endif
    }

    public void GetSearchJsonDataCallBack(string json)
    {
        Debuger.Log("GetSearchJsonDataCallBack:" + json);
        JSearchResponseData data = JsonUtility.FromJson<JSearchResponseData>(json);
        if (data != null && data.data != null)
        {
            CacheVoiceSearchData.Instance.FeedList = data.data;
        }
    }

    /// <summary>
    /// 获取热搜关键字
    /// </summary>
    public void GetHotWordsJsonData()
    {
        JGetHotWordsRequest request = new JGetHotWordsRequest();
        request.request = mURLHotWords;

        string requestJson = JsonUtility.ToJson(request);
        string u3dName = gameObject.name;
        string funcName = "GetHotWordsJsonDataCallBack";

        string[] tParam = new string[3];
        tParam[0] = u3dName;
        tParam[1] = funcName;
        tParam[2] = requestJson;

#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidInterface.CallAndroidFunction(mAndroidHttpRequest, tParam);
#elif UNITY_EDITOR
        JKeyWordsData data = new JKeyWordsData();
        data.hotKeywords = new string[4];
        data.hotKeywords[0] = "五十度灰";
        data.hotKeywords[1] = "钢铁侠";
        data.hotKeywords[2] = "蜘蛛侠";
        data.hotKeywords[3] = "变形金刚";
        CacheVoiceSearchData.Instance.HotWords = data;
#endif
    }

    public void GetHotWordsJsonDataCallBack(string json)
    {
        Debuger.Log("GetSearchJsonDataCallBack:" + json);
        JHotWordsData data = JsonUtility.FromJson<JHotWordsData>(json);
        if (data != null && data.data != null)
        {
            CacheVoiceSearchData.Instance.HotWords = data.data;
        }
    }

    /// <summary>
    /// 根据剧集或者合集id来获取详细数据
    /// </summary>
    /// <param name="videosetId"></param>
    public void GetVideoList(string videosetId, string goname, string funname)
    {
        JVideoSetRequest tmp = new JVideoSetRequest();
        tmp.request = mURLVideoList;
        tmp.tag = videosetId;
        JVideoSetParam jParam = new JVideoSetParam();
        jParam.setId = videosetId;
        tmp.param = jParam;


        string param = JsonUtility.ToJson(tmp);

        string[] tParam = new string[3];
        tParam[0] = goname;
        tParam[1] = funname;
        tParam[2] = param;

#if !UNITY_EDITOR && UNITY_ANDROID
		AndroidInterface.CallAndroidFunction ("httpRequest", tParam);
#endif
    }
    
    #region load data in editor
    public static string LoadJsonStrFromFile(string filename)
    {
        if (!File.Exists(Application.dataPath + "/Resources/" + filename))
        {
            return null;
        }

        StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + filename);
        if (sr == null)
        {
            return null;
        }
        string json = sr.ReadToEnd();
        return json;
    }
    #endregion

    //	public static final String URL_U3D_HOMEPAGE = "/u3d/homePage"; //首页
    //	public static final String URL_U3D_CATEGORY = "/u3d/category";  //分类列表
    //	public static final String URL_U3D_CATEGORY_VIDEO = "/u3d/category/video"; //分类中视频列表

    //public void httpRequest(String u3dObject, String method, String requestJson);
    //public void cancelHttpRequest(String tag);
    //public void imageRequest(String u3dObject, String method, String url, String suffix);
    //	requestJson格式：
    //	{
    //		“request”: string,  //请求的url
    //		“tag”: string, //标记，用户取消http请求
    //		“params”: {  } //JsonObject，具体请求参数key-value
    //	}
    //	responseJson格式：
    //	{
    //		"result_code": string, //"0"代表成功
    //		“error_msg“: string, //错误信息
    //		”data“: {
    //			"perpage": int, //分页加载时有这个值，代表每页返回多少条数据
    //			”offset“: string, //分页加载时有这个值，代表当前偏移量
    //			……
    //		}
    //	}
}