using MyJsonClasses;
using System.Collections;
using System.Collections.Generic;

namespace InstanceCache
{
    public class DataCenter
    {
        //大厅缓存数据
        private static HomeCacheData _homeCache = null;
        public static HomeCacheData HomeCache
        {
            get
            {
                if (_homeCache == null)
                {
                    _homeCache = new HomeCacheData();
                }
                return _homeCache;
            }
        }
    }

    // 大厅数据
    public class HomeCacheData
    {
        public JMainPageData MainPageData { get; set; }             //首页tab数据
        public JCategoryList CategoryList { get; set; }             //首页categories
        public HomeTabPageData TabPageCache { get; set; }
        public InfoData InfoPageCache { get; set; }
        public int firstOrderTabSelect = -1;
        public int secondOrderTabSelected = -1;
        public int secondOrdetTabOffset = 0;

        public void InitCategory(string jsondata)
        {
            if (string.IsNullOrEmpty(jsondata))
            {
                Debuger.LogError("the jsondata is null!");
                MainUIController.ShowMainUIMsg(Constant.DATA_LOAD_ERROR);
                return;
            }
            JTab tabClass = UnityEngine.JsonUtility.FromJson<JTab>(jsondata);
            if (tabClass != null && tabClass.data != null && tabClass.result_code == "0")
            {
                JCategoryList list = tabClass.data;
                CategoryList = new JCategoryList();
                // 首页 离线 历史
                CategoryList.categoryList = new JCategoryItem[list.categoryList.Length + 3];

                // 添加首页
                CategoryList.categoryList[0] = createTabCategoryItem(Constant.TAB_MAIN_NAME, "homepage", null, null);
                //CategoryList.categoryList[0].title = Constant.TAB_MAIN_NAME;
                //CategoryList.categoryList[0].category = new JCategory();
                //CategoryList.categoryList[0].category.homePageLeftTopic = Constant.TAB_MAIN_NAME;

                // 添加网络数据
                for (int i = 1; i < list.categoryList.Length + 1; i++)
                {
                    CategoryList.categoryList[i] = list.categoryList[i - 1];
                }

                // 添加离线
                string[] tstr = new string[2];
                tstr[0] = Constant.TAB_DOWNLOAD_NAME;
                tstr[1] = Constant.TAB_LOCAL_NAME;
                string[] tpgStr = new string[2];
                tpgStr[0] = Constant.TAB_PAGECATEGORY_DOWNLOAD;
                tpgStr[1] = Constant.TAB_PAGECATEGORY_LOCAL;
                JCategoryItem tItem = createTabCategoryItem(Constant.TAB_OFFLINE_NAME, Constant.TAB_PAGECATEGORY_LOCAL, tstr, tpgStr);
                CategoryList.categoryList[list.categoryList.Length + 1] = tItem;

                // 添加历史
                string[] tstr2 = new string[0];
                string[] tpgStr2 = new string[0];
                JCategoryItem tItem1 = createTabCategoryItem(Constant.TAB_HISTORY_NAME, Constant.TAB_PAGECATEGORY_HISTORY, tstr2, tpgStr2);
                CategoryList.categoryList[list.categoryList.Length + 2] = tItem1;

                MsgManager.Instance.SendMsg(MsgID.UpdataCategoryData);
            }
            else
            {
                MainUIController.ShowMainUIMsg(Constant.DATA_LOAD_ERROR);
            }
        }

        private JCategoryItem createTabCategoryItem(string title, string pageCategory, string[] sTitle, string[] sPageCategory)
        {
            //Debuger.Assert(sTitle.Length == sPageCategory.Length);
            JCategoryItem categoryItem = new JCategoryItem();
            categoryItem.title = title;

            JCategory category = new JCategory();
            category.pageCategory = pageCategory;
            category.homePageLeftTopic = title;

            categoryItem.category = category;

            if (sTitle != null && sTitle.Length > 0)
            {
                JCategory[] sCategory = new JCategory[sTitle.Length];
                for (int i = 0; i < sCategory.Length; i++)
                {
                    sCategory[i] = new JCategory();
                    sCategory[i].pageCategory = sPageCategory[i];
                    sCategory[i].homePageLeftTopic = sTitle[i];
                }
                categoryItem.secondary = sCategory;
            }
            else
            {
                categoryItem.secondary = new JCategory[0];
            }

            return categoryItem;
        }
    }

    // 分类页数据
    public class HomeTabPageData
    {
        public string PageCategory { get; set; }
        public string VRFormat { get; set; }
        public string PageOffset { get; set; }
        public ArrayList VideoItemList { get; set; }
        public HashSet<string> FilterIDHashSet { get; set; }
        public int CurrentPageNum { get; set; }
        public int CurrentPageIndex { get; set; }
        public bool NeedLoad { get; set; }
        public bool NoMoreData { get; set; }
    }
}